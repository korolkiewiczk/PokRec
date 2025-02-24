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
        public class NodeData
        {
            public float[] Cfr { get; set; }
            public float[] Strategy { get; set; }
            public float[] StrategySum { get; set; }
            
            internal readonly object Lock = new();

            public NodeData()
            {
            }

            public NodeData(int actions)
            {
                Cfr = new float[actions];
                Strategy = new float[actions];
                StrategySum = new float[actions];
            }
        }

        // Cache the mask for this node (assuming Round doesn't change)
        private readonly int _cachedMask;

        public Node(int pos, PlayerAction action, Round round, Node[] children, int payOff)
        {
            Pos = pos;
            Action = action;
            Round = round;
            Children = children;
            PayOff = payOff;
            _cachedMask = ComputeMask();
            Data = new();
        }

        public int Pos { get; set; }
        public PlayerAction Action { get; set; }
        public Round Round { get; set; }
        public Node[] Children { get; set; }
        public int PayOff { get; set; }

        public ConcurrentDictionary<int, NodeData> Data { get; set; }

        public static bool IsTerminal(Round round) => round is Round.Fold or Round.Showdown;
        public bool IsTerminal() => IsTerminal(Round);

        public override string ToString()
        {
            return $"P={Pos} A={Action} S={Round}" + (IsTerminal() ? $" PAY={PayOff}" : "");
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

        public int ComputeMask()
        {
            // The bitmask ((16 << (4 * (int)Round)) - 1) determines how many bits to keep.
            return ((16 << (4 * (int) Round)) - 1);
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
            return Data.GetOrAdd(maskedHand, _ => new NodeData(Children.Length));
        }
    }
}