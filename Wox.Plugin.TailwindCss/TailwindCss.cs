using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Algolia.Search;
using Newtonsoft.Json.Linq;

namespace Wox.Plugin.Our.Umbraco
{
    internal class Main : IPlugin
    {
        public Main()
        {
        }

        public void Init(PluginInitContext context)
        {
        }
        
        public string Truncate(string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length) + "...";
            }
            var result = Regex.Replace(source, @"\r\n?|\n", " ");
            return result;
        }

        public List<Result> Query(Query query)
        {
            var algolia = new AlgoliaClient("BH4D9OD16A", "3df93446658cd9c4e314d4c02a052188");
            var index = algolia.InitIndex("tailwindcss");
            var search = query.Search;

            if (search != "")
            {
                var results = index.Search(new Algolia.Search.Query(search).SetNbHitsPerPage(5));

                var result = new List<Result>();

                foreach (var item in results["hits"])
                {
                    var title = "";
                    var subtitle = "";

                    var levels = item["hierarchy"].Children<JProperty>().OrderBy(x => x.Name).Reverse();

                    foreach (var level in levels)
                    {
                        if (level != null)
                        {
                            if (title == "")
                            {
                                title = level.Value.ToString();
                            }
                            else
                            {
                                subtitle = level.Value.ToString() + (subtitle != "" ? " - " + subtitle : "");
                            }
                        }
                    }

                    if (title != "")
                    {
                        result.Add(new Result()
                        {
                            Title = title,
                            SubTitle = subtitle,
                            IcoPath = "tailwind.png",
                            Action = c =>
                            {
                                try
                                {
                                    Process.Start(item["url"].ToString());
                                    return true;
                                }
                                catch (ExternalException e)
                                {
                                    MessageBox.Show("Open failed, please try later");
                                    return false;
                                }
                            }
                        });
                    }
                }

                return result;
            }
            else
            {
                return new List<Result>();
            }
        }
    }
}