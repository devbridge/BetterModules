using System;
using System.IO;
using Microsoft.AspNet.FileProviders;

namespace BetterModules.Core.Web.Web.EmbeddedResources
{
    public class NotFoundFileInfo: IFileInfo
    {
        public NotFoundFileInfo(string name)
        {
            Name = name;
        }

        public bool Exists => false;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified => DateTimeOffset.MinValue;

        public long Length => -1;

        public string Name { get; }

        public string PhysicalPath => null;

        public Stream CreateReadStream()
        {
            throw new FileNotFoundException($"The file {Name} does not exist.");
        }
    }
}