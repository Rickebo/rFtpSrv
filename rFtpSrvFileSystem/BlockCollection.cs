using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace rFtpSrvFileSystem
{
    public class BlockCollection
    {
        private readonly Dictionary<FileSystemInfoWrapper, BlockInfo> _blocks;
        private readonly List<RegexBlockEntry> _regexBlocks;

        public BlockCollection()
        {
            _blocks = new Dictionary<FileSystemInfoWrapper, BlockInfo>();
            _regexBlocks = new List<RegexBlockEntry>();
        }

        public void AddBlock(FileSystemInfo file, BlockInfo block) => _blocks.Add(new FileSystemInfoWrapper(file), block);
        public void AddBlock(FileSystemInfo file, BlockOperation operation) => _blocks.Add(new FileSystemInfoWrapper(file), BlockInfoExtensions.SetBlock(operation));

        public void AddBlock(Regex regex, BlockInfo blockInfo) => _regexBlocks.Add(new RegexBlockEntry(regex, blockInfo));
        public void AddBlock(Regex regex, BlockOperation operation) => _regexBlocks.Add(new RegexBlockEntry(regex, BlockInfoExtensions.SetBlock(operation)));

        public bool IsBlocked(FileSystemInfo file, BlockOperation operation)
        {
            if (_blocks.TryGetValue(new FileSystemInfoWrapper(file), out var blockInfo))
                return blockInfo.IsBlocked(operation);

            var filename = file.FullName;
            foreach (var entry in _regexBlocks)
            {
                if (!entry.IsMatch(filename))
                    continue;

                return entry.BlockInfo.IsBlocked(operation);
            }

            return false;
        }

        private class RegexBlockEntry
        {
            public readonly Regex Regex;
            public readonly BlockInfo BlockInfo;

            public RegexBlockEntry(Regex regex, BlockInfo blockInfo)
            {
                Regex = regex;
                BlockInfo = blockInfo;
            }

            public bool IsMatch(string filename) => Regex.IsMatch(filename);
        }
    }
}
