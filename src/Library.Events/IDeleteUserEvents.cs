﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Events
{
    public interface IDeleteUserEvents
    {
        public string UserId { get; set; }
    }
}
