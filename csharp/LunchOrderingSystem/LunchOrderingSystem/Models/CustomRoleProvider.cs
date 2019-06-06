using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace LunchOrderingSystem.Models
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string userId)
        {
            
            using (var db = new DatabaseContext())
            {
                int id = int.Parse(userId);
                var roles = db.m_user_role
                    .Where(u => u.m_user.id == id)
                    .Select(u => u.role)
                    .ToArray();

                if (roles != null)
                {
                    return roles;
                }
            }
            return new string[] { "Unknown" };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string userId, string roleName)
        {
            using (var db = new DatabaseContext())
            {
                int id = int.Parse(userId);
                var user = db.m_user_role
                    .Where(u => u.m_user.id == id && u.role == roleName)
                    .FirstOrDefault();

                if (user != null)
                {
                    return true;
                }
            }

            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}