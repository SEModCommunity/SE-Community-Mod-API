var container, stats;
var camera, controls, scene, renderer;
var pickingData = [], pickingTexture, pickingScene;
var objects = [];
var highlightBox;

var mouse = new THREE.Vector2();
var offset = new THREE.Vector3( 10, 10, 10 );

var entityList = [];
var entitySceneMeshes = [];
var entitySceneObjects = [];

function webGLStart()
{
	init();
	animate();
	
	setInterval(function(){refreshEntities();}, 5000);
}

function getEntityId(entity)
{
	var rawEntityId = $(entity).children("entityid")[0];
	var entityId = parseInt(rawEntityId.innerHTML);

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

function getEntityMass(entity)
{
	var rawMass = $(entity).children("mass")[0];
	var mass = parseInt(rawMass.innerHTML);

	return mass;
}

function isVoxelMap(entity)
{
	return $(entity).children("a\\:filename").length > 0;
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

function updateEntity(entity)
{
	var entityId = getEntityId(entity);
	var geom = entitySceneObjects[entityId];
	var mesh = entitySceneMeshes[entityId];
	
	var matrix = new THREE.Matrix4();
	var quaternion = new THREE.Quaternion();
	
	var position = getEntityPosition(entity);
	var mass = getEntityMass(entity);

	var geomPosition = new THREE.Vector3();
	geomPosition.x = 0;
	geomPosition.y = 0;
	geomPosition.z = 0;

	var rotation = new THREE.Euler();
	rotation.x = Math.random() * 2 * Math.PI;
	rotation.y = Math.random() * 2 * Math.PI;
	rotation.z = Math.random() * 2 * Math.PI;

	var scale = new THREE.Vector3();
	scale.x = mass / 2000 + 10;
	scale.y = mass / 2000 + 10;
	scale.z = mass / 2000 + 10;

	quaternion.setFromEuler( rotation, false );
	matrix.compose( geomPosition, quaternion, scale );

	//geom.applyMatrix(matrix);
	//geom.verticesNeedUpdate = true;
	pickingData[ entityId ].position.set(position.x, position.y, position.z);
	mesh.position.set(position.x, position.y, position.z);
}

function addEntityToScene(entity)
{
	var geometry = new THREE.Geometry();
	var pickingGeometry = new THREE.Geometry();
	var pickingMaterial = new THREE.MeshBasicMaterial( { vertexColors: THREE.VertexColors } );
	var defaultMaterial = new THREE.MeshLambertMaterial({ color: 0xffffff, shading: THREE.FlatShading, vertexColors: THREE.VertexColors	} );

	var geom;
	if(!isVoxelMap(entity))
	{
		geom = new THREE.BoxGeometry( 1, 1, 1 );
	} else {
		geom = new THREE.SphereGeometry( 1, 32, 32 );
	}
	
	var color = new THREE.Color();

	var matrix = new THREE.Matrix4();
	var quaternion = new THREE.Quaternion();
	
	var entityId = getEntityId(entity);
	var position = getEntityPosition(entity);
	var mass = getEntityMass(entity);
	
	var geomPosition = new THREE.Vector3();
	geomPosition.x = 0;
	geomPosition.y = 0;
	geomPosition.z = 0;

	var rotation = new THREE.Euler();
	rotation.x = Math.random() * 2 * Math.PI;
	rotation.y = Math.random() * 2 * Math.PI;
	rotation.z = Math.random() * 2 * Math.PI;

	var scale = new THREE.Vector3();
	scale.x = mass / 2000 + 10;
	scale.y = mass / 2000 + 10;
	scale.z = mass / 2000 + 10;

	quaternion.setFromEuler( rotation, false );
	matrix.compose( geomPosition, quaternion, scale );
	applyVertexColors( geom, color.setHex( Math.random() * 0xffffff ) );
	geometry.merge( geom, matrix );

	applyVertexColors( geom, color.setHex( entityId ) );
	matrix.compose( position, quaternion, scale );
	pickingGeometry.merge( geom, matrix );

	pickingData[ entityId ] = {
		position: position,
		rotation: rotation,
		scale: scale
	};
	
	var drawnObject = new THREE.Mesh( geometry, defaultMaterial );
	drawnObject.position.set(position.x, position.y, position.z);
	scene.add( drawnObject );
	
	pickingScene.add( new THREE.Mesh( pickingGeometry, pickingMaterial ) );
	
	entitySceneObjects[ entityId ] = geometry;
	entitySceneMeshes[ entityId ] = drawnObject;
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

	var light = new THREE.SpotLight( 0xffffff, 1.5 );
	light.position.set( 0, 500, 2000 );
	scene.add( light );

	highlightBox = new THREE.Mesh(
		new THREE.BoxGeometry( 1, 1, 1 ),
		new THREE.MeshLambertMaterial( { color: 0xffff00 }
	) );
	scene.add( highlightBox );

	projector = new THREE.Projector();

	renderer = new THREE.WebGLRenderer( { antialias: true } );
	renderer.setClearColor( 0xffffff, 1 );
	renderer.setSize( renderWidth, renderHeight );
	renderer.sortObjects = false;
	container.appendChild( renderer.domElement );

	renderer.domElement.addEventListener( 'mousemove', onMouseMove );
	
	refreshEntities();
}

function onMouseMove(e)
{
	var rectObject = container.getBoundingClientRect();
	mouse.x = e.clientX + rectObject.left + 10;
	mouse.y = e.clientY + rectObject.top + 10;
}

function onWindowResize()
{
	var renderWidth = parseInt(container.style.width.split("px")[0]);
	var renderHeight = parseInt(container.style.height.split("px")[0]);
	
	camera.aspect = renderWidth / renderHeight;
	camera.updateProjectionMatrix();

	renderer.setSize( renderWidth, renderHeight );
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

	var gl = self.renderer.getContext();

	//read the pixel under the mouse from the texture

	var pixelBuffer = new Uint8Array( 4 );
	gl.readPixels( mouse.x, pickingTexture.height - mouse.y, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, pixelBuffer );

	//interpret the pixel as an ID

	var id = ( pixelBuffer[0] << 16 ) | ( pixelBuffer[1] << 8 ) | ( pixelBuffer[2] );
	var data = pickingData[ id ];

	if ( data)
	{
		//move our highlightBox so that it surrounds the picked object

		if ( data.position && data.rotation && data.scale )
		{
			highlightBox.position.copy( data.position );
			highlightBox.rotation.copy( data.rotation );
			highlightBox.scale.copy( data.scale ).add( offset );
			highlightBox.visible = true;
		}
	} else {
		highlightBox.visible = false;
	}
}

function render()
{
	controls.update();

	pick();

	renderer.render( scene, camera );
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