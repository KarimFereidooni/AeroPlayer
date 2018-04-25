using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AeroPlayer
{
    class AssemblyInformation
    {
        #region Assembly Attribute Accessors
        
        private Assembly ExecutingAssembly;

        public AssemblyInformation(Assembly ExecutingAssembly)
        {
            this.ExecutingAssembly = ExecutingAssembly;
        }
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = this.ExecutingAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(this.ExecutingAssembly.CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return this.ExecutingAssembly.GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = this.ExecutingAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = this.ExecutingAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = this.ExecutingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = this.ExecutingAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        public string AssemblyLocation
        {
            get
            {
                return this.ExecutingAssembly.Location;
            }
        }
        public string AssemblyTitle_And_version
        {
            get
            {
                return this.AssemblyTitle + " نسخه " + this.AssemblyVersion;
            }
        }
        #endregion
    }
}
