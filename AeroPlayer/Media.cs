using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AeroPlayer
{
    public class Media
    {
        public Media(string FilePath, string Name)
        {
            this.FilePath = FilePath;
            this.Name = Name;
        }
        public Media(string FilePath)
        {
            this.FilePath = FilePath;
            this.Name = System.IO.Path.GetFileNameWithoutExtension(FilePath);
        }
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        private string _FilePath;
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                _FilePath = value;
            }
        }
        public override string ToString()
        {
            return this.Name;
        }
        public override bool Equals(object obj)
        {
            if (obj is Media)
            {
                if (((Media)obj).FilePath.ToLower() == this.FilePath.ToLower())
                    return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int hashcode = 0;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(this.FilePath);
            foreach (byte item in bytes)
            {
                hashcode += item;
            }
            return hashcode;
        }
    }
}
