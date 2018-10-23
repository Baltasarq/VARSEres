namespace VARSEres.Core {
    using System.Collections.Generic;

    public class Result {
        /// <summary>Base class for events.</summary>
        public abstract class Event {
            public enum EventType { Beat, Tag };

            /// <summary>The millis this event happened at.</summary>
            public long Time {
                get; set;
            }

            /// <summary>The type of event.</summary>
            public EventType Type {
                get; set;
            }

            /// <summary>The contents in this event.</summary>
            public object Content {
                get; set;
            }

            public override string ToString()
            {
                return this.Type + " (@" + this.Time + "): " + this.Content;
            }
        }

        /// <summary>Base class for events.</summary>
        public abstract class Event<T>: Event {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public T Value {
                get {
                    return (T) base.Content;
                }
                set {
                    base.Content = value;
                }
            }
		}

        /// <summary>A heart beat event.</summary>
        public class BeatEvent: Event<long> {
            public BeatEvent(long time, long beat)
            {
                this.Time = time;
                this.Type = EventType.Beat;
                this.Value = beat;
            }
        }

        /// <summary>A tag change event.</summary>
        public class TagEvent: Event<string> {
            public TagEvent(long time, string tag)
            {
                this.Time = time;
                this.Type = EventType.Tag;
                this.Value = tag;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VARSEres.Core.Result"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
		/// <param name="date">The date (as long).</param>
        /// <param name="time">The total time for the experiment (as long).</param>
		/// <param name="name">The name (as string, normally it contains data about the experiment and user).</param>
        /// <param name="usrId">User's id.</param>
        /// <param name="exprId">Experiment's id.</param>
        /// <param name="events">Events.</param>
        public Result(Id id, long date, string name, long time, Id exprId, Id usrId, IEnumerable<Event> events)
        {
			this.Id = id;
			this.Date = date;
			this.name = name;
            this.Time = time;
            this.UsrId = usrId;
            this.ExperimentId = exprId;
            this.events = new List<Event>( events );
        }

        /// <summary>Gets the events.</summary>
        /// <value>The events, as an array of <see cref="Event"/>.</value>
        public Event[] Events {
            get {
                return this.events.ToArray();
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The <see cref="Id"/>.</value>
        public Id Id {
            get; private set;
        }
        
        /// <summary>
        /// Gets the experiment identifier.
        /// </summary>
        /// <value>The experiment <see cref="Id"/>.</value>
        public Id ExperimentId {
            get; private set;
        }
        
        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <value>The user <see cref="Id"/>.</value>
        public Id UsrId {
            get; private set;
        }

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <value>The date, as a long in millis.</value>
		public long Date {
			get; private set;
		}

        /// <summary>
        /// Gets the total time for this experiment.
        /// </summary>
        /// <value>The time, as a long in millis.</value>
        public long Time {
            get; private set;
        }

		string name;
        List<Event> events;
    }
}
