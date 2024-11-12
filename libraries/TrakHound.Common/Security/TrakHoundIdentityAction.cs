// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Security
{
    public enum TrakHoundIdentityActionType
    {
        Success,
        Redirect,
        Fail,
        Error,
        NotImplemented
    }

    public interface ITrakHoundIdentityAction
    {
        string Id { get; }

        TrakHoundIdentityActionType Type { get; }

        string Message { get; }
    }

    public abstract class TrakHoundIdentityActionBase : ITrakHoundIdentityAction
    {
        public string Id { get; }

        public virtual TrakHoundIdentityActionType Type { get; }

        public string Message { get; set; }


        public TrakHoundIdentityActionBase()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public sealed class TrakHoundIdentitySuccessAction : TrakHoundIdentityActionBase
    {
        public override TrakHoundIdentityActionType Type => TrakHoundIdentityActionType.Success;
    }

    public sealed class TrakHoundIdentityFailAction : TrakHoundIdentityActionBase
    {
        public override TrakHoundIdentityActionType Type => TrakHoundIdentityActionType.Fail;
    }

    public sealed class TrakHoundIdentityErrorAction : TrakHoundIdentityActionBase
    {
        public override TrakHoundIdentityActionType Type => TrakHoundIdentityActionType.Error;
    }

    public sealed class TrakHoundIdentityNotImplementedAction : TrakHoundIdentityActionBase
    {
        public override TrakHoundIdentityActionType Type => TrakHoundIdentityActionType.NotImplemented;
    }

    public sealed class TrakHoundIdentityRedirectAction : TrakHoundIdentityActionBase
    {
        public override TrakHoundIdentityActionType Type => TrakHoundIdentityActionType.Redirect;

        public string Location { get; set; }


        public TrakHoundIdentityRedirectAction(string location)
        {
            Location = location;
        }
    }
}
