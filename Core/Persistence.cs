namespace VARSEres.Core
{
	using System;
	using System.IO;
    using System.Json;
	using System.Collections.Generic;

    public static class Persistence
    {
		static string EtqId = "id";
		static string EtqName = "name";
		static string EtqDate = "date";
		static string EtqEvents = "events";
		static string EtqTime = "time";
		static string EtqValue = "value";
		static string EtqType = "type";

		static string TypeTagChange = "activity_change_event";
		static string TypeBeat = "beat_event";

        public static Result Load(string fileName)
        {
			long id = -1; 
			var events = new List<Result.Event>();
			string name = "";
			long date = -1;

			using (StreamReader r = new StreamReader( fileName ))
            {
                var json = r.ReadToEnd();
				var parsedJson = JsonObject.Parse( json );

				foreach (KeyValuePair<string, JsonValue> item in parsedJson)
				{
					if ( item.Key == EtqId ) {
						id = Convert.ToInt64( item.Value.ToString() );
					}
					else
					if ( item.Key == EtqName ) {
                        name = item.Value.ToString();
                    }
					else
                    if ( item.Key == EtqDate ) {
						date = Convert.ToInt64( item.Value.ToString() );
                    }
					else
                    if ( item.Key == EtqEvents ) {
    					if ( item.Value is JsonArray jEvents ) {
							foreach (JsonObject evt in jEvents ) {
								foreach(KeyValuePair<string, JsonValue> attr in evt) {
									long time = -1;
									string type = "";
									string value = "";

									if ( attr.Key == EtqTime ) {
										time = Convert.ToInt64( attr.Value.ToString() );
									}
									else
									if ( attr.Key == EtqType )
                                    {
                                        type = attr.Value;
                                    }
									else
                                    if ( attr.Key == EtqValue )
                                    {
                                        value = attr.Value;
                                    }
												
    								Result.Event.EventType evtType;

    								if ( type == TypeTagChange ) {
    									evtType = Result.Event.EventType.Tag;				
    								}
    								else
    								if ( type == TypeBeat )
                                    {
                                        evtType = Result.Event.EventType.Beat;
    								} else {
										throw new ArgumentException( "invalid event type: '" + type + "'" );
    								}
                                                                                     
        							events.Add( new Result.Event {
    								    Time = time,
    								    Type = evtType,
    									Value = value
        							});
								}
							}
    					}
                    }
                }
            }

            // Chk
			if ( id == -1 ) {
                throw new ArgumentException( "missing id" );
			}

			if ( date == -1 ) {
				throw new ArgumentException( "missing date" );
			}

			return new Result( new Id( id ), date, name, events );
        }
    }
}
