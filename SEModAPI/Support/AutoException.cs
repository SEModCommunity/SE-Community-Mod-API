using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SEModAPI
{

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Enum used to store exception states, derived classes must initialize it
    /// See GameInstallationExceptionInfo for example
    /// </summary>
    public abstract class IExceptionState
    {
        protected IExceptionState(System.Enum State) { m_ExceptionState = (int)Convert.ChangeType(State, typeof(int)); }

        protected int m_ExceptionState;
        public int ExceptionState { get { return m_ExceptionState; } }

        //Must be replaced
        protected string[] StateRepresentation;

        public string StateToString() { return StateRepresentation[m_ExceptionState]; }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class to easily handle exceptions
    /// </summary>
    [Serializable]
    public class AutoException : Exception
    {
        #region "Attributes"

        //Members
        private IExceptionState m_ExceptionState;
        private string m_AdditionnalInfo;

        //Properties
        /// <summary>
        /// Get instance of IExceptionState derived classe
        /// </summary>
        public IExceptionState ExceptionState { get { return m_ExceptionState; } }

        /// <summary>
        /// Get the CustomMessage
        /// </summary>
        public string AdditionnalInfo { get { return m_AdditionnalInfo; } }

        /// <summary>
        /// Get the Object context as string
        /// </summary>
        public string Object { get { return base.TargetSite.ReflectedType.FullName; } }

        /// <summary>
        /// Get the Method context as string
        /// </summary>
        public string Method { get { return base.TargetSite.Name; } }


        #endregion

        #region "Constructors & Initializers"

        /// <summary>
        /// Class to handle exceptions to ease debbuging by handling their context
        /// </summary>
        /// <param name="ExceptionState">An instance of a derived class of IExceptionState</param>
        /// <param name="CustomMessage">Additionnal informations about the exception</param>
        public AutoException(IExceptionState ExceptionState, string AdditionnalInfo = ""): base()
        {
            m_ExceptionState = ExceptionState;
            m_AdditionnalInfo = AdditionnalInfo;
        }

        /// <summary>
        /// The serialization constructor of AutoExceotion
        /// </summary>
        protected AutoException(SerializationInfo info, StreamingContext context): base(info, context){}

        #endregion

        #region "Methods"

        /// <summary>
        /// Method intended to return a standardized and simplified information about exceptions
        /// of the API to ease the debugging
        /// </summary>
        /// <returns>Simplified</returns>
        public string GetDebugString() { return Object + "->" + Method + "# " + m_ExceptionState.StateToString() + "; " + m_AdditionnalInfo; }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info == null)
                throw new System.ArgumentNullException("info");
            info.AddValue("Object", Object);
            info.AddValue("Method", Method);
            info.AddValue("AdditionnalInfo", m_AdditionnalInfo);
            info.AddValue("ExceptionState", m_ExceptionState.StateToString());
        }

        #endregion

    }
}
