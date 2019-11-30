using System;
using System.Collections.Generic;
using System.Text;

namespace rFtpSrvFileSystem
{
    public struct BlockInfo
    {
        private BlockOperation _operation;

        public void EnableBlock(BlockOperation operation)
        {
            _operation |= operation;
        }

        public void DisableBlock(BlockOperation operation)
        {
            _operation &= ~operation;
        }

        public bool IsBlocked(BlockOperation operation)
        {
            return (_operation & operation) != 0;
        }
    }

    public static class BlockInfoExtensions
    {
        // As a function extension to allow the two methods to "have the same arguments"
        public static BlockInfo SetBlock(this BlockInfo info, BlockOperation operation, bool state = true)
        {
            if (state)
                info.EnableBlock(operation);
            else
                info.DisableBlock(operation);

            return info;
        }

        public static BlockInfo SetBlock(BlockOperation operation, bool state = true)
        {
            return SetBlock(new BlockInfo(), operation, state);
        }
    }

    [Flags]
    public enum BlockOperation : byte
    {
        List = 1,

        Access = 16,
        Read = 17,
        Write = 18,

        All = byte.MaxValue
    }
}
