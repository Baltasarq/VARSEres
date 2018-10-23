namespace VARSEres.Core {
    using System.Collections.Generic;

    public class Result {
        /// <summary>The millis this event happened at.</summary>
        public class Event {
            public enum EventType { Beat, Tag };

            /// <summary>The millis this event happened at.</summary>
            public long Time {
                get; private set;
            }

            /// <summary>The type of event.</summary>
            public EventType Type {
                get; private set;
            }

            public string Value {
                get; private set;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VARSEres.Core.Result"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="events">Events.</param>
        public Result(Id id, IEnumerable<Event> events)
        {
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

        List<Event> events;
    }
}
