var container, previewContainer;
var camera, controls, scene, renderer;
var previewCamera, previewScene, previewRenderer;
var pickingData = [], pickingTexture, pickingScene;
var objects = [];
var highlightBox;

var mouse = new THREE.Vector2();
var offset = new THREE.Vector3( 10, 10, 10 );

var entityList = [];
var entitySceneMeshes = [];
var entityPickingSceneMeshes = [];
var entitySceneObjects = [];
var entitySceneArrows = [];
var cubeBlockList = [];
var previewMesh;
var previewObject;

function webGLStart()
{
	init();
	initPreview();
	animate();
	
	setInterval(function(){refreshEntities();}, 1000);
}

function getEntityId(entity)
{
	var rawEntityId = $(entity).children("entityid")[0];
	var entityId = rawEntityId.innerHTML;

	return entityId;
}

function getEntityPosition(entity)
{
	var rawPosition = $(entity).children("position");
	var x = parseFloat(rawPosition.children()[0].innerHTML);
	var y = parseFloat(rawPosition.children()[1].innerHTML);
	var z = parseFloat(rawPosition.children()[2].innerHTML);
	
	var position = new THREE.Vector3();
	position.x = x;
	position.y = y;
	position.z = z;

	return position;				
}

function getEntityLinearVelocity(entity)
{
	var rawVelocity = $(entity).children("linearvelocity");
	var x = parseFloat(rawVelocity.children()[0].innerHTML);
	var y = parseFloat(rawVelocity.children()[1].innerHTML);
	var z = parseFloat(rawVelocity.children()[2].innerHTML);
	
	var velocity = new THREE.Vector3();
	velocity.x = x;
	velocity.y = y;
	velocity.z = z;

	return velocity;				
}

function getEntityMass(entity)
{
	var rawMass = $(entity).children("mass")[0];
	var mass = parseInt(rawMass.innerHTML);

	return mass;
}

function getEntityName(entity)
{
	var rawName = $(entity).children("name")[0];
	
	return rawName;
}

function isCubeGrid(entity)
{
	return $(entity).children("a\\:gridsizeenum").length > 0;
}

function isVoxelMap(entity)
{
	return $(entity).children("a\\:filename").length > 0;
}

function getCubeBlockPosition(cubeBlock)
{
	var rawPosition = $(cubeBlock).children("min");
	var x = parseFloat(rawPosition.children()[0].innerHTML);
	var y = parseFloat(rawPosition.children()[1].innerHTML);
	var z = parseFloat(rawPosition.children()[2].innerHTML);
	
	var position = new THREE.Vector3();
	position.x = x;
	position.y = y;
	position.z = z;

	return position;				
}

function getCubeBlockColor(cubeBlock)
{
	var rawColor = $(cubeBlock).children("colormaskhsv");
	var h = (parseFloat(rawColor.children()[0].innerHTML) + 1) / 2;
	var s = (parseFloat(rawColor.children()[1].innerHTML) + 1) / 2;
	var v = (parseFloat(rawColor.children()[2].innerHTML) + 1) / 2;
	
	var color = new THREE.Color();
	color.setHSL(h, s, v);

	return color;				
}

function applyVertexColors( g, c )
{
	g.faces.forEach( function( f ) {
		var n = ( f instanceof THREE.Face3 ) ? 3 : 4;

		for( var j = 0; j < n; j ++ )
		{
			f.vertexColors[ j ] = c;
		}
	} );
}

function reloadGeometry()
{
	//Sync up entities
	for (var key in entityList)
	{
		var entity = entityList[key];
		var entityId = getEntityId(entity);
		if(typeof entitySceneObjects[entityId] != "undefined" && entitySceneObjects[entityId] != null)
		{
			updateEntity(entity);
		} else {
			addEntityToScene(entity);
		}
	}
	
	//Remove any geometries linked to entities that are no longer in the list
	for (var key in entitySceneObjects)
	{
		if(typeof entityList[key] == "undefined" || entityList[key] == null)
		{
			var geom = entitySceneObjects[key];
			geom.dispose();
		}
	}
	
	renderer.render( scene, camera );
}

function clearGeometry()
{
	for (var key in entitySceneObjects)
	{
		var geom = entitySceneObjects[key];
		geom.dispose();
	}
}

function packEntityId(entityId)
{
	var result = entityId & 0x0000000000ffffff;
	return result;
}

function updateEntity(entity)
{
	var entityId = getEntityId(entity);
	var position = getEntityPosition(entity);
	var linearVelocity = getEntityLinearVelocity(entity);

	var mesh = entitySceneMeshes[entityId];
	var pickingMesh = entityPickingSceneMeshes[entityId];
	var arrowHelper = entitySceneArrows[entityId];

	pickingData[ packEntityId(entityId) ].position.set(position.x, position.y, position.z);
	
	mesh.position.set(position.x, position.y, position.z);
	pickingMesh.position.set(position.x, position.y, position.z);
	
	if(linearVelocity.length() > 1 && isCubeGrid(entity))
	{
		if(!arrowHelper)
		{
			var length = linearVelocity.length();
			var hex = 0x00ff00;
			arrowHelper = new THREE.ArrowHelper( linearVelocity.normalize(), position, length, hex );
			scene.add( arrowHelper );
			entitySceneArrows[ entityId ] = arrowHelper;
		}
		
		arrowHelper.setDirection(linearVelocity.normalize());
		arrowHelper.position.set(position.x, position.y, position.z);
	} else {
		if(arrowHelper)
		{
			arrowHelper.dispose();
			entitySceneArrows[ entityId ] = null;
		}
	}
}

function addEntityToScene(entity)
{
	var geometry = new THREE.Geometry();
	var pickingGeometry = new THREE.Geometry();
	var pickingMaterial = new THREE.MeshBasicMaterial( { vertexColors: THREE.VertexColors } );
	var defaultMaterial = new THREE.MeshLambertMaterial({ color: 0xffffff, shading: THREE.FlatShading, vertexColors: THREE.VertexColors	} );

	var entityId = getEntityId(entity);
	var position = getEntityPosition(entity);
	var linearVelocity = new THREE.Vector3();
	var mass = 0;
	
	var geom;
	if(isVoxelMap(entity))
	{
		geom = new THREE.SphereGeometry( 1, 32, 32 );
		mass = 1000000;
	}
	if(isCubeGrid(entity))
	{
		geom = new THREE.BoxGeometry( 1, 1, 1 );
		mass = getEntityMass(entity);
		linearVelocity = getEntityLinearVelocity(entity);
	}
	if(!geom)
	{
		geom = new THREE.CylinderGeometry( 1, 1, 1 );
		mass = 10;
	}
	
	var color = new THREE.Color();

	var matrix = new THREE.Matrix4();
	var quaternion = new THREE.Quaternion();
	
	var geomPosition = new THREE.Vector3();
	geomPosition.x = 0;
	geomPosition.y = 0;
	geomPosition.z = 0;

	var rotation = new THREE.Euler();
	rotation.x = Math.random() * 2 * Math.PI;
	rotation.y = Math.random() * 2 * Math.PI;
	rotation.z = Math.random() * 2 * Math.PI;

	var scale = new THREE.Vector3();
	scale.x = Math.log(mass) * 7 + 1;
	scale.y = Math.log(mass) * 7 + 1;
	scale.z = Math.log(mass) * 7 + 1;

	//Finish building the position-orientation matrix
	quaternion.setFromEuler( rotation, false );
	matrix.compose( geomPosition, quaternion, scale );
	
	applyVertexColors( geom, color.setHex( Math.random() * 0xffffff ) );
	geometry.merge( geom, matrix );

	applyVertexColors( geom, color.setHex( packEntityId(entityId) ) );
	pickingGeometry.merge( geom, matrix );

	pickingData[ packEntityId(entityId) ] = {
		entityId: entityId,
		position: position,
		rotation: rotation,
		scale: scale
	};
	
	var drawnObject = new THREE.Mesh( geometry, defaultMaterial );
	drawnObject.position.set(position.x, position.y, position.z);
	scene.add( drawnObject );
	
	var arrowHelper;
	if(linearVelocity.length() > 1 && isCubeGrid(entity))
	{
		var length = linearVelocity.length();
		var hex = 0x00ff00;
		var arrowHelper = new THREE.ArrowHelper( linearVelocity.normalize(), position, length, hex );
		scene.add( arrowHelper );
	}
	
	var pickingDrawnObject = new THREE.Mesh( pickingGeometry, pickingMaterial );
	pickingDrawnObject.position.set(position.x, position.y, position.z);
	pickingScene.add( pickingDrawnObject );
	
	entitySceneObjects[ entityId ] = geometry;
	entitySceneMeshes[ entityId ] = drawnObject;
	entityPickingSceneMeshes[ entityId ] = pickingDrawnObject;
	entitySceneArrows[ entityId ] = arrowHelper;
}

function reloadPreviewGeometry()
{
	if(previewObject)
		previewObject.dispose();
		
	previewObject = new THREE.Geometry();
	var defaultMaterial = new THREE.MeshPhongMaterial({ color: 0xffffff, shading: THREE.SmoothShading, vertexColors: THREE.VertexColors	} );
	//defaultMaterial.wireframe = true;
	
	var farthestCubePos = new THREE.Vector3();
	for(var key in cubeBlockList)
	{
		var cubeBlock = cubeBlockList[key];
		var geom = new THREE.BoxGeometry( 1, 1, 1 );
	
		var color = getCubeBlockColor(cubeBlock);
		//color.offsetHSL(0, 0.3, 0.3);

		var matrix = new THREE.Matrix4();
		var quaternion = new THREE.Quaternion();
	
		var geomPosition = getCubeBlockPosition(cubeBlock);
		if(geomPosition.length() > farthestCubePos.length())
			farthestCubePos.copy(geomPosition);

		var rotation = new THREE.Euler();
		rotation.x = 0;
		rotation.y = 0;
		rotation.z = 0;

		var scale = new THREE.Vector3();
		scale.x = 1;
		scale.y = 1;
		scale.z = 1;

		//Finish building the position-orientation matrix
		quaternion.setFromEuler( rotation, false );
		matrix.compose( geomPosition, quaternion, scale );
		
		applyVertexColors( geom, color );
		previewObject.merge( geom, matrix );
	}
	
	previewMesh = new THREE.Mesh( previewObject, defaultMaterial );
	var center = previewObject.center();
	previewMesh.position.set(center.x, center.y, center.z);
	previewScene.add( previewMesh );
	
	previewCamera.position.x = 0;
	previewCamera.position.y = center.y + 5;
	previewCamera.position.z = 0 + farthestCubePos.length() + 10;
	previewCamera.lookAt(new THREE.Vector3());
}

function init()
{
	container = document.getElementById( "container" );
	var renderWidth = parseInt(container.style.width.split("px")[0]);
	var renderHeight = parseInt(container.style.height.split("px")[0]);

	camera = new THREE.PerspectiveCamera( 70, renderWidth / renderHeight, 1, 200000 );
	camera.position.z = 1000;

	controls = new THREE.TrackballControls( camera );
	controls.rotateSpeed = 1.0;
	controls.zoomSpeed = 1.2;
	controls.panSpeed = 0.8;
	controls.noZoom = false;
	controls.noPan = false;
	controls.staticMoving = true;
	controls.dynamicDampingFactor = 0.3;

	scene = new THREE.Scene();

	pickingScene = new THREE.Scene();
	pickingTexture = new THREE.WebGLRenderTarget( renderWidth, renderHeight );
	pickingTexture.generateMipmaps = false;

	scene.add( new THREE.AmbientLight( 0x555555 ) );

	var light = new THREE.DirectionalLight( 0xffffff, 1.5 );
	light.position.set( 0, 20000, 20000 );
	scene.add( light );

	highlightBox = new THREE.Mesh(
		new THREE.BoxGeometry( 1, 1, 1 ),
		new THREE.MeshLambertMaterial( { color: 0xffff00 }
	) );
	scene.add( highlightBox );

	projector = new THREE.Projector();

	renderer = new THREE.WebGLRenderer( { antialias: true } );
	renderer.setClearColor( 0x000000, 1 );
	renderer.setSize( renderWidth, renderHeight );
	renderer.sortObjects = false;
	container.appendChild( renderer.domElement );

	renderer.domElement.addEventListener( 'mousemove', onMouseMove );
	
	refreshEntities();
}

function initPreview()
{
	previewContainer = document.getElementById( "preview" );
	var renderWidth = parseInt(previewContainer.style.width.split("px")[0]);
	var renderHeight = parseInt(previewContainer.style.height.split("px")[0]);

	previewCamera = new THREE.PerspectiveCamera( 70, renderWidth / renderHeight, 1, 1000 );
	previewCamera.position.z = 50;

	previewScene = new THREE.Scene();
	
	previewScene.add( new THREE.AmbientLight( 0x555555 ) );
	
	var light = new THREE.DirectionalLight( 0xffffff, 1.5 );
	light.position.set( 0, 50, 200 );
	previewScene.add( light );
	
	previewRenderer = new THREE.WebGLRenderer( { antialias: true } );
	previewRenderer.setClearColor( 0xffffff, 1 );
	previewRenderer.setSize( renderWidth, renderHeight );
	previewRenderer.sortObjects = false;
	previewContainer.appendChild( previewRenderer.domElement );
}

function onMouseMove(e)
{
	var rectObject = container.getBoundingClientRect();
	mouse.x = e.clientX - rectObject.left;
	mouse.y = e.clientY - rectObject.top;
}

function animate()
{
	requestAnimationFrame( animate );

	render();
}

function pick()
{
	//render the picking scene off-screen
	renderer.render( pickingScene, camera, pickingTexture );

	var gl = renderer.getContext();

	//read the pixel under the mouse from the texture
	var pixelBuffer = new Uint8Array( 4 );
	gl.readPixels( mouse.x, pickingTexture.height - mouse.y, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, pixelBuffer );

	//interpret the pixel as an ID
	var id = ( pixelBuffer[0] << 16 ) | ( pixelBuffer[1] << 8 ) | ( pixelBuffer[2] );
	var data = pickingData[ id ];

	if (data)
	{
		//move our highlightBox so that it surrounds the picked object
		if ( data.position && data.rotation && data.scale )
		{
			var entity = entityList[data.entityId];
			if(!highlightBox.visible && isCubeGrid(entity))
			{
				refreshPreview(entity);
			}
			
			highlightBox.position.copy( data.position );
			highlightBox.rotation.copy( data.rotation );
			highlightBox.scale.copy( data.scale ).add( offset );
			highlightBox.visible = true;
			
			var mass = 0;
			var name = getEntityName(entity);
			var linearVelocity = new THREE.Vector3();
			if(isCubeGrid(entity))
			{
				mass = getEntityMass(entity);
				linearVelocity = getEntityLinearVelocity(entity);
			}
			
			$("#entityIdCell").html(data.entityId);
			$("#nameCell").html(name);
			$("#positionCell").html(data.position.x + "," + data.position.y + "," + data.position.z);
			$("#linearVelocityCell").html(linearVelocity.x + "," + linearVelocity.y + "," + linearVelocity.z);
			$("#massCell").html(mass);
			
			if(previewMesh)
				previewMesh.rotation.y -= 0.005;
		}
	} else {
		if(highlightBox.visible && previewObject && previewMesh)
		{
			previewMesh.visible = false;
			previewObject.dispose();
			previewObject = null;
			previewMesh = null;
		}
		
		highlightBox.visible = false;
		
		$("#entityIdCell").html("");
		$("#nameCell").html("");
		$("#positionCell").html("");
		$("#linearVelocityCell").html("");
		$("#massCell").html("");
	}
}

function render()
{
	controls.update();

	pick();

	renderer.render( scene, camera );
	previewRenderer.render( previewScene, previewCamera );
}

function make_base_auth(user, password)
{
	var tok = user + ':' + password;
	var hash = btoa(tok);
	var authString = 'Basic ' + hash;
	console.log("Auth: '" + authString + "'");
	return authString;
}

function refreshEntities()
{
	var serviceURL = "http://localhost:8000/SEServerExtender/Web/";
	var getEntitiesMethod = "GetSectorEntities";
	entityList = [];
	$.ajax({
		url: serviceURL + getEntitiesMethod,
		type: "GET",
		crossDomain: true,
		success: function(data){
			var childNodes = $(data).children().children();
			$.each(childNodes, function(index, value) {
				var entityId = getEntityId(value);
				entityList[ entityId ] = value;
			});
			reloadGeometry();
		},
		error: function( jqXHR, textStatus, errorThrown ) {
			console.log(jqXHR);
			console.log(errorThrown);
		}
	});
}

function refreshPreview(entity)
{
	var entityId = getEntityId(entity);
	var serviceURL = "http://localhost:8000/SEServerExtender/Web/";
	var wcfMethod = "GetCubeBlocks/" + entityId;
	cubeBlockList = [];
	$.ajax({
		url: serviceURL + wcfMethod,
		type: "GET",
		crossDomain: true,
		success: function(data){
			var childNodes = $(data).children().children();
			$.each(childNodes, function(index, value) {
				cubeBlockList.push(value);
			});
			reloadPreviewGeometry();
		},
		error: function( jqXHR, textStatus, errorThrown ) {
			console.log(jqXHR);
			console.log(errorThrown);
		}
	});
}