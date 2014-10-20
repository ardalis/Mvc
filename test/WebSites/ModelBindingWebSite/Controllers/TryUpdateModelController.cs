// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace ModelBindingWebSite.Controllers
{
    public class TryUpdateModelController : Controller
    {
        public async Task<User> GetUserAsync(int id)
        {
            var user = GetUserFromSomeWhere(id);
            var bindingContext = await BindingContextProvider.GetActionBindingContextAsync(ActionContext);
            await TryUpdateModelAsync(user,
                                      string.Empty,
                                      new[] { "RegisterationMonth" },
                                      new[] { "Id", "Key" },
                                      bindingContext.ValueProvider);

            return user;
        }

        public async Task<User> GetUserAsync_IncludeListNull(int id)
        {
            var user = GetUserFromSomeWhere(id);
            await TryUpdateModelAsync(user,
                                      string.Empty,
                                      null,
                                      new[] { "Id", "Key" });

            return user;
        }

        public async Task<User> GetUserAsync_ExcludeListNull(int id)
        {
            var user = GetUserFromSomeWhere(id);
            await TryUpdateModelAsync(user,
                                      string.Empty,
                                      new[] { "RegisterationMonth" });

            return user;
        }

        public async Task<User> GetUserAsync_IncludeAndExcludeListNull(int id)
        {
            var user = GetUserFromSomeWhere(id);
            await TryUpdateModelAsync(user);

            return user;
        }

        private User GetUserFromSomeWhere(int id)
        {
            return new User
            {
                UserName = "User_" + id,
                Id = id,
                Key = id + 20,
            };
        }
    }

    public class User
    {
        public int Id { get; set; }
        public int Key { get; set; }
        public string RegisterationMonth { get; set; }
        public string UserName { get; set; }
    }
}