﻿// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using Nancy;
using System.Linq;
using System;
using System.Collections.Generic;

namespace AlarmWorkflow.BackendService.WebService.Modules
{
    public class Settings : NancyModule
    {
        #region Constants

        private readonly string[] SettingsWhitelist = { "WebService", "Shared" };

        #endregion

        #region Fields

        private IServiceProvider _serviceProvider;

        #endregion

        #region Constructors

        public Settings(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var settingsService = serviceProvider.GetService<ISettingsServiceInternal>();

            Get["/api/settings"] = parameter =>
            {
                SettingsDisplayConfiguration configuration = settingsService.GetDisplayConfiguration();
                return Response.AsJson(configuration.Identifiers);
            };

            Get["/api/settings/{name}"] = parameter =>
            {
                SettingsDisplayConfiguration configuration = settingsService.GetDisplayConfiguration();
                IdentifierInfo identifier = configuration.Identifiers.Find(_ => _.Name == parameter.name);

                if (identifier == null)
                {
                    return HttpStatusCode.NoContent;
                }
                if(!SettingsWhitelist.Contains(identifier.Name))
                {
                    //authorization not supported yet. Send 403 if not in whitelist.
                    return 403;
                }
                
                Dictionary<string, Object> settingsDic = new Dictionary<string, Object>();

                foreach (SettingInfo info in identifier.Settings)
                {
                    SettingItem setting = settingsService.GetSetting(info.CreateSettingKey());
                    settingsDic.Add(setting.Name, setting.Value);
                }

                return Response.AsJson(settingsDic);
            };
        }

        #endregion
    }
}
