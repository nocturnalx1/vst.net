﻿namespace Jacobi.Vst.Framework.Plugin
{
    using Jacobi.Vst.Core;
    using Jacobi.Vst.Core.Deprecated;
    using System;

    /// <summary>
    /// Provides a base for the plugin root class.
    /// </summary>
    /// <remarks>Derive your plugin root class from this base class when you want to implement the
    /// Interface Manager at plugin root class level.</remarks>
    public abstract class VstPluginWithInterfaceManagerBase : Plugin.PluginInterfaceManagerBase, IVstPlugin
    {
        /// <summary>
        /// To be called from the default constructor of the derived plugin class
        /// to initialize the base class.
        /// </summary>
        /// <param name="name">The name of the plugin. Must not be null. <seealso cref="Name"/></param>
        /// <param name="productInfo">The product information of the plugin. Must not be null. <seealso cref="ProductInfo"/></param>
        /// <param name="category">The plugin category. <seealso cref="Category"/></param>
        /// <param name="capabilities">The plugin capabilities <seealso cref="Capabilities"/>.</param>
        /// <param name="initialDelay">The initial delay of the plugin. <seealso cref="InitialDelay"/></param>
        /// <param name="pluginID">The unique Id of the plugin. <seealso cref="PluginID"/></param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="productInfo"/> or <paramref name="name"/> is not set to an instance of an object.</exception>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="name"/> is an empty string.</exception>
        protected VstPluginWithInterfaceManagerBase(string name, VstProductInfo productInfo,
            VstPluginCategory category, VstPluginCapabilities capabilities,
            int initialDelay, int pluginID)
        {
            Throw.IfArgumentIsNull(productInfo, nameof(productInfo));
            Throw.IfArgumentIsNullOrEmpty(name, nameof(name));

            ProductInfo = productInfo;
            Name = name;
            Category = category;
            Capabilities = capabilities;
            InitialDelay = initialDelay;
            PluginID = pluginID;
        }

        #region IVstPlugin Members

        /// <summary>
        /// Retrieves product information for the plugin (instance).
        /// </summary>
        public VstProductInfo ProductInfo { get; private set; }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the category of the plugin.
        /// </summary>
        public VstPluginCategory Category { get; private set; }

        /// <summary>
        /// Gets the additional capabilities of the plugin.
        /// </summary>
        public VstPluginCapabilities Capabilities { get; private set; }

        /// <summary>
        /// Gets the initial delay (in samples??)
        /// </summary>
        public int InitialDelay { get; private set; }

        /// <summary>
        /// Gets the unique identifier of the plugin represented as a 4 character code.
        /// </summary>
        public int PluginID { get; private set; }

        /// <summary>
        /// The host will call this method when the plugin is loaded and should open its resources.
        /// </summary>
        /// <param name="host">A reference to the Host root interface. This reference can be used to 
        /// query for other host interfaces.Must not be null.</param>
        /// <remarks>Open is a good time to allocate large memory blocks when you use a pre-allocated 
        /// memory schema in your plugin.</remarks>
        public virtual void Open(IVstHost host)
        {
            Host = host;

            OnOpened();
        }

        /// <summary>
        /// Called by the host when the user has turned off your plugin.
        /// </summary>
        /// <remarks>Your plugin is NOT being unloaded by the host. The user just turned it off.</remarks>
        public virtual void Suspend()
        {
            // no-op
        }

        /// <summary>
        /// Called by the host when the user has turned on your plugin.
        /// </summary>
        /// <remarks>Your plugin can receive multiple <see cref="Suspend"/>/Resume calls during its lifetime.</remarks>
        public virtual void Resume()
        {
            if (Supports<IVstMidiProcessor>())
            {
                IVstHostCommandsDeprecated20 deprecated = Host.GetInstance<IVstHostCommandsDeprecated20>();

                if (deprecated != null)
                {
                    deprecated.WantMidi();
                }
            }
        }

        #endregion

        /// <summary>
        /// Called by the framework to cleanup the plugin resources.
        /// </summary>
        /// <param name="disposing">When true, release (dispose) both managed and unmanaged resource. When false, only release unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Host = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the reference to the Host root object.
        /// </summary>
        /// <remarks>This member can be null. It is set after a call to <see cref="Open"/>.</remarks>
        public IVstHost Host { get; private set; }

        /// <summary>
        /// Triggered when the <see cref="M:Open"/> method is called.
        /// </summary>
        /// <remarks>At this point the <see cref="P:Host"/> property is available.</remarks>
        public event EventHandler Opened;

        /// <summary>
        /// Triggers the <see cref="Opened"/> event.
        /// </summary>
        protected virtual void OnOpened()
        {
            Opened?.Invoke(this, EventArgs.Empty);
        }
    }
}
