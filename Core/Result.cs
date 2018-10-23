namespace VARSEres.Core {
    using System.Collections.Generic;

    public class Result {
        /// <summary>The millis this event happened at.</summary>
        public class Event {
            public enum EventType { Beat, Tag };

            /// <summary>The millis this event happened at.</summary>
            public long Time {
                get; set;
            }

            /// <summary>The type of event.</summary>
            public EventType Type {
                get; set;
            }

            public string Value {
                get; set;
            }

			public override string ToString()
			{
				return this.Type + " (@" + this.Time + "): " + this.Value;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VARSEres.Core.Result"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
		/// <param name="date">The date (as long).</param>
		/// <param name="name">The name (as string, normally it contains data about the experiment and user).</param>
        /// <param name="events">Events.</param>
        public Result(Id id, long date, string name, IEnumerable<Event> events)
        {
			this.Id = id;
			this.Date = date;
			this.name = name;
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
        /// Gets the date.
        /// </summary>
        /// <value>The date, as a long in millis.</value>
		public long Date {
			get; private set;
		}

		string name;
        List<Event> events;
    }
}
