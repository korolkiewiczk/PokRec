﻿using Common;

namespace Game.RegionMatchers
{
    public class Turn : CardsMatcher
    {
        public Turn(Board board) : base(board)
        {
        }

        public override RegionSpec GetRegionSpec()
        {
            var spec = base.GetRegionSpec();
            spec.Num = 1;
            return spec;
        }
    }
}