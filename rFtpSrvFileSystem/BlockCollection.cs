using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace rFtpSrvFileSystem
{
    public class BlockCollection
    {
        private readonly Dictionary<FileSystemInfoWrapper, BlockInfo> _blocks;

        public BlockCollection()
        {
            _blocks = new Dictionary<FileSystemInfoWrapper, BlockInfo>();
        }

        public void AddBlock(FileSystemInfo file, BlockInfo block) => _blocks.Add(new FileSystemInfoWrapper(file), block);
        public void AddBlock(FileSystemInfo file, BlockOperation operation) => _blocks.Add(new FileSystemInfoWrapper(file), BlockInfoExtensions.SetBlock(operation));

        public bool IsBlocked(FileSystemInfo file, BlockOperation operation) 
            => _blocks.TryGetValue(new FileSystemInfoWrapper(file), out var blockInfo) && blockInfo.IsBlocked(operation);
    }
}
