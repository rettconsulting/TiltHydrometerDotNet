﻿/*
 * Copyright 2015-2016 Huysentruit Wouter
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;

namespace WinBeacon.Stack
{
    /// <summary>
    /// Queue extension methods.
    /// </summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// Dequeue all items from the queue.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="queue">The queue.</param>
        /// <returns>Array of dequeued items.</returns>
        public static T[] DequeueAll<T>(this Queue<T> queue)
        {
            var result = new List<T>();
            while (queue.Count > 0)
                result.Add(queue.Dequeue());
            return result.ToArray();
        }

        /// <summary>
        /// Dequeue the specified number of items.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="queue">The queue.</param>
        /// <param name="count">Number of items to dequeue.</param>
        /// <returns>Array of dequeued items.</returns>
        public static T[] Dequeue<T>(this Queue<T> queue, int count)
        {
            var result = new List<T>();
            for (int i = 0; i < count; i++)
                result.Add(queue.Dequeue());
            return result.ToArray();
        }

        /// <summary>
        /// Enqueue an array of items.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="queue">The queue.</param>
        /// <param name="items">The items.</param>
        public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (T item in items)
                queue.Enqueue(item);
        }
    }
}
