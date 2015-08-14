using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using HRNUG.Models;
using HRNUG.ViewModels;
using Newtonsoft.Json;

namespace HRNUG.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var vm = new HomeViewModel();

            // Attempt to load meetup events
            try
            {
                await LoadUpcomingEvents(vm);
            }
            catch
            {
                vm.Error = "There was a problem retrieving events. Please visit our meetup.";
            }

            return View(vm);
        }

        /// <summary>
        /// Grabs upcoming events form Meetup to display
        /// </summary>
        /// <param name="vm">The ViewModel to load the events into</param>
        /// <returns>Async Task</returns>
        private async Task LoadUpcomingEvents(HomeViewModel vm)
        {
            // Grab upcoming meetups
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.meetup.com/");

                var response =
                    await
                        client.GetAsync("2/events?key=3514476181f3663732595436e1347&sign=true&group_id=4338572&status=upcoming");

                if (response.IsSuccessStatusCode)
                {
                    // Use a dynamic json object to get return values from
                    var events = new List<Event>();
                    dynamic json = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                    // Add each event from meetup
                    foreach (var result in json.results)
                    {
                        // Get :: out of event name
                        string title = result.name;
                        if (title.Contains("::"))
                            title = title.Split(new[] {"::"}, StringSplitOptions.None)[1].Trim();

                        // Convert from epoc time using milliseconds sent from API
                        var utcDateTime =
                            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((double) result.time);
                        var time = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime,
                            TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

                        events.Add(new Event()
                        {
                            Title = title,
                            DateTime = time,
                            Link = string.Format("http://www.meetup.com/Hampton-Roads-NET-Users-Group/events/{0}/", result.id)
                        });
                    }

                    vm.Events = events;
                }
            }
        }
    }
}