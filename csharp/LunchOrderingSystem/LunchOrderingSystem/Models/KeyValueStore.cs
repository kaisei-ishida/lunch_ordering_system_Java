using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace LunchOrderingSystem.Models
{
    public class KeyValueStore : DynamicObject
    {
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            using (var db = new DatabaseContext())
            {
                var item = db.key_values
                    .Where(k => k.key == binder.Name)
                    .FirstOrDefault();

                if(item == null)
                {
                    var newItem = new key_values { key = binder.Name, value = value.ToString() };
                    db.key_values.Add(newItem);
                    db.SaveChanges();
                }
                else
                {
                    item.value = value.ToString();
                    db.SaveChanges();
                }
            }
            return true;
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            using (var db = new DatabaseContext())
            {
                var item = db.key_values
                    .Where(k => k.key == binder.Name)
                    .FirstOrDefault();

                if(item != null)
                {
                    result = item.value;
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
        }
    }
}