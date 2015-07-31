using System.Collections.Generic;
using HRNUG.Models;

namespace HRNUG.ViewModels
{
    public class HomeViewModel
    {
        public string Error { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }
}