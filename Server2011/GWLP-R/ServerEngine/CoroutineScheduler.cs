using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading;

namespace ServerEngine
{
        /// <summary>
        ///   This class enables the usage of coroutine-like workarounds for C#
        /// </summary>
        public sealed class CoroutineScheduler
        {
                private readonly object objLock = new object();
                private readonly List<IEnumerator> listBackend;

                // BUG: this will only work in this context!
                private List<IEnumerator> coroutineList
                {
                        get { lock (objLock) return listBackend; }
                }

                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                public CoroutineScheduler()
                {
                        lock (objLock)
                                listBackend = new List<IEnumerator>();
                }

                /// <summary>
                ///   Adds a new coroutine to the schedule
                /// </summary>
                /// <param name="coroutine"></param>
                public void Add(IEnumerator coroutine)
                {
                        coroutineList.Add(coroutine);
                }

                /// <summary>
                ///   The scheduler task
                /// </summary>
                public void Update()
                {
                        /* You can use this the following way:
                        * - yield null: do nothing
                        * - yield a TimeSpan: the function is paused.
                        * - yield an IEnumerator: add a new function.
                        * - yield 'true': end this function and remove it from the list
                        */
                        for (var i = 0; i < coroutineList.Count(); i++)
                        {

                                var cor = coroutineList[i];

                                // this executes the next part of the coroutine
                                cor.MoveNext();

                                // #1 do nothing if the function returns null
                                if (cor.Current == null) continue;

                                // #2 check if the function wants to be paused
                                if (cor.Current is TimeSpan)
                                {
                                        var time = (TimeSpan)(cor.Current);

                                        // temporarily remove the enumerator from the list,
                                        // and add it after time has passed
                                        //
                                        // remove the coroutine from the list
                                        coroutineList.RemoveAt(i);

                                        // create a new timer (the timer will call TimerAdd after TimeSpan passed,
                                        new Timer((x => Add((IEnumerator)x)), cor, time,
                                                        TimeSpan.FromMilliseconds(-1));
                                }

                                // #3 check if we need to add a new coroutine
                                if (cor.Current is IEnumerator)
                                {
                                        // this may happen if an effect of a skill has complex sub effects
                                        Add((IEnumerator)cor.Current);
                                }

                                // #4 check if the function has finished (cor.Current is true in this case)
                                if (cor.Current is bool && (bool)cor.Current)
                                {
                                        coroutineList.RemoveAt(i);
                                }

                        }
                }
        }
}