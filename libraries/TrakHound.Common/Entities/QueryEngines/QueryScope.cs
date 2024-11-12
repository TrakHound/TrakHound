// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities.Collections;

namespace TrakHound.Entities.QueryEngines
{
    public class QueryScope
    {
        public List<QueryStatement> Statements { get; set; }

        public List<QueryVariable> Variables { get; set; }


        public QueryScope() 
        {
            Statements = new List<QueryStatement>();
            Variables = new List<QueryVariable>();
        }


        public QueryVariable GetVariable(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return Variables?.FirstOrDefault(o => o.Name == name);
            }

            return null;
        }

        public void SetVariable(QueryVariable variable)
        {
            if (variable != null && !string.IsNullOrEmpty(variable.Name))
            {
                Variables.RemoveAll(o => o.Name == variable.Name);
                Variables.Add(variable);
            }
        }


        public async Task<TrakHoundQueryResult[]> Execute(ITrakHoundSystemEntitiesClient client)
        {
            if (client != null && !Statements.IsNullOrEmpty())
            {
                var results = new List<TrakHoundQueryResult>();
                for (var i = 0; i < Statements.Count; i++)
                {
                    var statementResults = await Statements[i].Execute(client);
                    if (!statementResults.IsNullOrEmpty()) results.AddRange(statementResults);
                }
                return results.ToArray();
            }

            return null;
        }

        public TrakHoundQueryResult[] Execute(TrakHoundEntityCollection collection)
        {
            if (collection != null && !Statements.IsNullOrEmpty())
            {
                var results = new List<TrakHoundQueryResult>();
                for (var i = 0; i < Statements.Count; i++)
                {
                    var statementResults = Statements[i].Execute(collection);
                    if (!statementResults.IsNullOrEmpty()) results.AddRange(statementResults);
                }
                return results.ToArray();
            }

            return null;
        }

        public static QueryScope Create(string query)
        {
            var scope = new QueryScope();

            if (!string.IsNullOrEmpty(query))
            {
                var q = RemoveComments(query);

                var statementQueries = q.Split(';', StringSplitOptions.RemoveEmptyEntries);
                if (!statementQueries.IsNullOrEmpty())
                {
                    var statements = new List<QueryStatement>();

                    foreach (var statementQuery in statementQueries)
                    {
                        statements.Add(QueryStatement.Create(scope, statementQuery));
                    }

                    scope.Statements = statements;
                }
            }

            return scope;
        }

        private static string RemoveComments(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var outputLines = new List<string>();
                var lines = query.Split("\r\n");
                foreach (var line in lines)
                {
                    if (!line.StartsWith("--"))
                    {
                        var match = Regex.Match(line, @"(?=--)(.*)");
                        if (match.Success)
                        {
                            outputLines.Add(line.Remove(match.Index, match.Length));
                        }
                        else
                        {
                            outputLines.Add(line);
                        }
                    }
                }

                return string.Join("\r\n", outputLines);
            }

            return null;
        }
    }
}
