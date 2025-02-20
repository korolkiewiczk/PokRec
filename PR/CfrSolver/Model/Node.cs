using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CfrSolver.Model
{
    /// <summary>
    /// Represents a node in the game tree.
    /// This implementation is fully thread safe by grouping all per-hand data
    /// into a structure and protecting it with a per-key lock.
    /// </summary>
    public class Node
    {
        // Private class that holds the per-masked-hand data.
        public class NodeData(int actions)
        {
            public readonly float[] Cfr = new float[actions];
            public readonly float[] Strategy = new float[actions];
            public readonly float[] StrategySum = new float[actions];
            [JsonIgnore]
            public readonly object Lock = new();
        }

        // Dictionary mapping the masked hand value to its NodeData.
        private readonly ConcurrentDictionary<int, NodeData> _data = new();

        // Cache the mask for this node (assuming Round doesn't change)
        private readonly int _cachedMask;

        public Node(byte pos, PlayerAction action, Round round, Node[] children, int payOff = 0)
        {
            Pos = pos;
            Action = action;
            Round = round;
            Children = children;
            PayOff = (short)payOff;
            _cachedMask = ComputeMask();
        }

        public byte Pos { get; }
        public PlayerAction Action { get; }
        public Round Round { get; }
        public Node[] Children { get; }
        public short PayOff { get; }

        public ConcurrentDictionary<int, NodeData> Data => _data;

        public static bool IsTerminal(Round round) => round is Round.Fold or Round.Showdown;
        public bool IsTerminal() => IsTerminal(Round);

        public override string ToString()
        {
            return $"P={Pos} A={Action} S={Round}" + (IsTerminal() ? $" PAY={PayOff}" : "");
        }

        public string ToStringFull(int hand)
        {
            string part1 = ToString();
            string part2 = string.Join(";", GetAverageStrategy(hand));
            return $"{part1} [{part2}]";
        }

        /// <summary>
        /// Gets the current strategy for a given hand and updates the cumulative strategy sum.
        /// </summary>
        public float[] GetStrategy(int hand, float realizationWeight)
        {
            NodeData data = GetOrCreateData(hand);
            lock (data.Lock)
            {
                float normalizingSum = 0;
                // Compute the current strategy from CFR values.
                for (int a = 0; a < Children.Length; a++)
                {
                    data.Strategy[a] = data.Cfr[a] > 0 ? data.Cfr[a] : 0;
                    normalizingSum += data.Strategy[a];
                }

                // Normalize and update the cumulative strategy sum.
                for (int a = 0; a < Children.Length; a++)
                {
                    data.Strategy[a] = normalizingSum > 0
                        ? data.Strategy[a] / normalizingSum
                        : 1.0f / Children.Length;
                    data.StrategySum[a] += realizationWeight * data.Strategy[a];
                }

                // Return a copy so the caller doesn't modify the internal array.
                float[] result = new float[Children.Length];
                Array.Copy(data.Strategy, result, Children.Length);
                return result;
            }
        }

        /// <summary>
        /// Gets the average strategy for the specified hand.
        /// </summary>
        public float[] GetAverageStrategy(int hand)
        {
            NodeData data = GetOrCreateData(hand);
            lock (data.Lock)
            {
                float normalizingSum = 0;
                float[] avgStrategy = new float[Children.Length];
                for (int a = 0; a < Children.Length; a++)
                {
                    normalizingSum += data.StrategySum[a];
                }
                for (int a = 0; a < Children.Length; a++)
                {
                    avgStrategy[a] = normalizingSum > 0
                        ? data.StrategySum[a] / normalizingSum
                        : 1.0f / Children.Length;
                }
                return avgStrategy;
            }
        }

        /// <summary>
        /// Updates the CFR (counterfactual regret) for the specified action.
        /// </summary>
        public void UpdateCfr(int hand, int actionIndex, float delta)
        {
            NodeData data = GetOrCreateData(hand);
            lock (data.Lock)
            {
                data.Cfr[actionIndex] += delta;
                // Ensure regret values remain non-negative (CFR+)
                data.Cfr[actionIndex] = Math.Max(0, data.Cfr[actionIndex]);
                // Update the corresponding strategy value.
                data.Strategy[actionIndex] = data.Cfr[actionIndex];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetMaskedHand(int hand)
        {
            return hand & _cachedMask;
        }

        /// <summary>
        /// Retrieves or creates the NodeData for the given hand.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private NodeData GetOrCreateData(int hand)
        {
            int maskedHand = GetMaskedHand(hand);
            return _data.GetOrAdd(maskedHand, _ => new NodeData(Children.Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ComputeMask()
        {
            // The bitmask ((16 << (4 * (int)Round)) - 1) determines how many bits to keep.
            return ((16 << (4 * (int)Round)) - 1);
        }
    }
}
