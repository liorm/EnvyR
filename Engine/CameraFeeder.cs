using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvyR.Server.Services;
using Splat;

namespace EnvyR.Server.Engine
{
    class CameraFeeder : IEnableLogger
    {
        public static CameraFeeder Create()
        {
            var result = new CameraFeeder();
            result.Start();
            return result;
        }

        private CameraFeeder()
        {
            
        }

        private async void Start()
        {
            this.Log().Debug("Before delay");
            await Task.Delay(new Random().Next(1000, 3000));
            this.Log().Debug("after delay");

            Locator.Current.GetService<IApp>().Quit();
        }
    }
}
