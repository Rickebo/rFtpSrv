using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

        [Pure]
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
    public enum BlockOperation : ushort
    {
        List = 1,

        Create = 2,
        Delete = 2 << 1,

        Read = 2 << 2,
        Write = 2 << 3,

        Move = 2 | 2 << 1 | 2 << 2 | 2 << 3,
        Copy = 2 | 2 << 2 | 2 << 3,

        Access = 2 << 4,
        Unlink = 2 << 5,
        Link = 2 << 6,

        SetTime = 2 << 7,

        All = ushort.MaxValue
    }
}