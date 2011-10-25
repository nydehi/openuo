using System;
using System.Net;
using System.Windows;

namespace Client
{
    public interface IUpdate
    {
        void Update(UpdateState state);
    }
}
