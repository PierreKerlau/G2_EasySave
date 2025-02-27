using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Livrable1.Model;

namespace Livrable1.ViewModel
{
    class MainViewModel
    {
        private Server _server;

        public MainViewModel()
        {
            _server = Server.Instance; 
        }
    }
}