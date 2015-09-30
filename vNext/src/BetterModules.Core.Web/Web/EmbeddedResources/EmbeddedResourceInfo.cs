using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNet.FileProviders;

namespace BetterModules.Core.Web.Web.EmbeddedResources
{
    public class EmbeddedResourceInfo: IFileInfo
    {
        private readonly Assembly _assembly;
        private readonly string _resourcePath;

        private long? _length;

        public EmbeddedResourceInfo(Assembly assembly, string resourcePath, string name, DateTimeOffset lastModified)
        {
            _assembly = assembly;
            LastModified = lastModified;
            _resourcePath = resourcePath;
            Name = name;
        }

        public bool Exists => true;

        public long Length
        {
            get
            {
                if (!_length.HasValue)
                {
                    using (Stream stream = _assembly.GetManifestResourceStream(_resourcePath))
                    {
                        _length = stream.Length;
                    }
                }
                return _length.Value;
            }
        }

        // Not directly accessible.
        public string PhysicalPath => null;

        public string Name { get; }

        public DateTimeOffset LastModified { get; }

        public bool IsDirectory => false;

        public Stream CreateReadStream()
        {
            Stream stream = _assembly.GetManifestResourceStream(_resourcePath);
            if (!_length.HasValue)
            {
                _length = stream.Length;
            }
            return stream;
        }
    }
}