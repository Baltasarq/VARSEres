﻿namespace VARSEres.Core
{
	using System;
	using System.IO;
    using System.Json;
	using System.Collections.Generic;

    public static class Persistence
    {
		static string EtqId = "_id";
        static string EtqExperimentId = "experiment_id";
        static string EtqUsrId = "user_id";
        static string EtqTypeId = "type_id";
        static string EtqTime = "time";
		static string EtqName = "name";
		static string EtqDate = "date";
		static string EtqEvents = "events";
        static string EtqElapsedTime = "elapsed_time";
		static string EtqType = "event_type";
        static string EtqTag = "tag";
        static string EtqBeatAt = "heart_beat_at";

		static string TypeTagChange = "event_activity_change";
        static string TypeBeat = "event_heart_beat";
        static string TypeResult = "result";

        static Result.Event LoadEvent(JsonObject evt)
        {
            long time = -1;
            string type = "";
            object value = null;
            Result.Event toret = null;

            foreach(KeyValuePair<string, JsonValue> attr in evt) {
                if ( attr.Key == EtqElapsedTime ) {
                    time = attr.Value;
                }
                else
                if ( attr.Key == EtqType ) {
                    type = attr.Value;
                }
                else
                if ( attr.Key == EtqBeatAt ) {
                    value = (long) attr.Value;
                }
                else
                if ( attr.Key == EtqTag ) {
                    value = (string) attr.Value;
                }
            }

            // Chk
            if ( time == -1 ) {
                throw new ArgumentException( "missing time in event" );
            }

            if ( value == null ) {
                throw new ArgumentException( "missing value in event" );
            }

            // Decide event type
            if ( type == TypeTagChange ) {
                if ( value is string tag ) {
                    toret = new Result.TagEvent( time, tag );
                } else {
                    throw new ArgumentException( "not a tag: " + value );
                }
            }
            else
            if ( type == TypeBeat ) {
                if ( value is long beat ) {
                    toret = new Result.BeatEvent( time, beat );
                } else {
                    throw new ArgumentException( "not a beat rr: " + value );
                }
            } else {
                throw new ArgumentException( "invalid event type: '" + type + "'" );
            }

            return toret;
        }

        public static Result Load(string fileName)
        {
			long id = -1; 
            long usrId = -1;
            long exprId = -1;
			var events = new List<Result.Event>();
			string name = "";
            string typeId = "";
			long date = -1;
            long time = -1;

            try {
    			using (StreamReader r = new StreamReader( fileName ))
                {
                    var json = r.ReadToEnd();
    				var parsedJson = JsonValue.Parse( json );

    				foreach (KeyValuePair<string, JsonValue> item in parsedJson)
    				{
    					if ( item.Key == EtqId ) {
    						id = item.Value;
    					}
    					else
                        if ( item.Key == EtqTypeId ) {
                            typeId = item.Value;
                        }
                        else
    					if ( item.Key == EtqName ) {
                            name = item.Value;
                        }
    					else
                        if ( item.Key == EtqDate ) {
    						date = item.Value;
                        }
                        else
                        if ( item.Key == EtqTime ) {
                            time = item.Value;
                        }
                        else
                        if ( item.Key == EtqExperimentId ) {
                            exprId = item.Value;
                        }
                        else
                        if ( item.Key == EtqUsrId ) {
                            usrId = item.Value;
                        }
    					else
                        if ( item.Key == EtqEvents ) {
        					if ( item.Value is JsonArray jEvents ) {
    							foreach (JsonObject evt in jEvents ) {
                                    events.Add( LoadEvent( evt ) );
    							}
        					}
                        }
                    }
                }
            } catch(FormatException exc) {
                throw new ArgumentException( "format error: " + exc.Message );    
            }

            // Chk
            if ( string.IsNullOrWhiteSpace( typeId )
              || typeId != TypeResult )
            {
                throw new ArgumentException( "not a result" );
            }

			if ( id == -1 ) {
                throw new ArgumentException( "missing id" );
			}

			if ( date == -1 ) {
				throw new ArgumentException( "missing date" );
			}

            if ( time == -1 ) {
                throw new ArgumentException( "missing total time" );
            }

			return new Result( new Id( id ),
                               date, name, time,
                               new Id( exprId ),
                               new Id( usrId ),
                               events );
        }
    }
}
