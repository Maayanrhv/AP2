using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ImageService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            //string logName = GetServiceNameAppConfig("LogName");
            //string parameter = logName + "\" \"" + logName;
            string parameter = "ImageServiceLog\" \"ImageServiceLog";
            Context.Parameters["assemblypath"] = "\"" + Context.Parameters["assemblypath"] + "\" \"" + parameter + "\"";
            base.OnBeforeInstall(savedState);
        }
        public string GetServiceNameAppConfig(string serviceName)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetAssembly(typeof(ServiceInstaller)).Location);
            return config.AppSettings.Settings[serviceName].Value;
        }
    }
}
