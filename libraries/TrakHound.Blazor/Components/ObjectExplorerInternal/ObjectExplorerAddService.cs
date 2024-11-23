// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectExplorerAddService
    {
        private readonly ObjectExplorerService _explorerService;
        private readonly AddData _data = new AddData();
        private AddMode _mode;
        private bool _isEdit;
        private bool _modalVisible;
        private bool _modalLoading;


        public bool IsEdit => _isEdit;

        public class AddData
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public string ContentType { get; set; }
            public string DefinitionId { get; set; }
            public string ImportJson { get; set; }
        }

        public enum AddMode
        {
            Basic,
            Advanced,
            Occurrence,
            Json
        }


        public bool ModalVisible => _modalVisible;

        public bool ModalLoading => _modalLoading;

        public AddMode Mode => _mode;

        public AddData Data => _data;


        public event EventHandler AddClicked;


        public ObjectExplorerAddService(ObjectExplorerService explorerService)
        {
            _explorerService = explorerService;
        }


        public void SetMode(AddMode mode) => _mode = mode;


        public void AddObject(string path = null)
        {
            _data.Path = path;
            if (_data.ContentType == null) _data.ContentType = TrakHoundObjectContentTypes.Directory;
            _data.DefinitionId = null;
            
            _isEdit = false;
            OpenAddModal();
        }

        public void AddChildObject(ITrakHoundObjectEntity parentObject)
        {
            if (parentObject != null)
            {
                _data.Path = $"{parentObject.Namespace}:{parentObject.Path}/";
                if (_data.ContentType == null) _data.ContentType = TrakHoundObjectContentTypes.Directory;
                _data.DefinitionId = null;

                _isEdit = false;
                OpenAddModal();
            }
        }

        public void EditObject(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var client = _explorerService.GetClient();
                if (client != null)
                {
                    var obj = client.System.Entities.Objects.ReadByUuid(objectUuid).Result; // should be async?
                    if (obj != null)
                    {
                        EditObject(obj);
                    }
                }
            }
        }

        public void EditObject(ITrakHoundObjectEntity obj)
        {
            if (obj != null)
            {
                _isEdit = true;

                _data.Path = $"{obj.Namespace}:{obj.Path}";
                _data.Name = obj.Path;
                _data.ContentType = obj.ContentType;

                OpenAddModal();
            }
        }


        public void OpenAddModal()
        {
            _modalVisible = true;
            _explorerService.Update();
        }

        public void ModalConfirm()
        {
            _modalLoading = true;

            if (AddClicked != null) AddClicked.Invoke(this, EventArgs.Empty);

            _ = Task.Run(async () =>
            {
                if (!IsEdit)
                {
                    await Add();
                }
                else
                {
                    await Edit();
                }

                _modalLoading = false;
                _modalVisible = false;
                _explorerService.Update();
            });
        }

        public void ModalCancel()
        {
            _data.Path = null;
            _data.ContentType = TrakHoundObjectContentTypes.Directory;
            _data.DefinitionId = null;

            _modalLoading = false;
            _modalVisible = false;
            _explorerService.Update();
        }


        private async Task Add()
        {
            switch (Mode)
            {
                case AddMode.Basic:

                    await AddBasic();
                    break;

                case AddMode.Json:

                    await AddJson();
                    break;

                case AddMode.Advanced:

                    await AddAdvanced();
                    break;
            }
        }

        private async Task AddBasic()
        {
            var addResponse = await _explorerService.Client.Entities.PublishObject(Data.Path, Data.ContentType.ConvertEnum<TrakHoundObjectContentType>(), Data.DefinitionId, async: false);
            if (addResponse != null)
            {
                var entity = await _explorerService.Client.System.Entities.Objects.ReadByUuid(addResponse.Uuid);
                if (entity != null)
                {
                    await _explorerService.AddObject(entity);
                    _explorerService.SelectObject(entity.Uuid);

                    _explorerService.AddNotification(NotificationType.Information, "Object Added Successfully", addResponse.GetAbsolutePath());
                }
                else
                {
                    _explorerService.AddNotification(NotificationType.Error, "Error Reading Entity", addResponse.Uuid);
                }
            }
            else
            {
                _explorerService.AddNotification(NotificationType.Error, "Error Adding Entity", Data.Path);
            }
        }

        private async Task AddJson()
        {
            if (await _explorerService.Client.Entities.PublishJson(Data.Path, Data.ImportJson))
            {
                var entity = (await _explorerService.Client.System.Entities.Objects.Query(Data.Path))?.FirstOrDefault();
                if (entity != null)
                {
                    await _explorerService.AddObject(entity);
                    _explorerService.SelectObject(entity.Uuid);

                    _explorerService.AddNotification(NotificationType.Information, "JSON Import Successful", entity.GetAbsolutePath());
                }
                else
                {
                    _explorerService.AddNotification(NotificationType.Error, "Error Reading Entity", Data.Path);
                }
            }
            else
            {
                _explorerService.AddNotification(NotificationType.Error, "Error Adding Entity", Data.Path);
            }
        }

        private async Task AddAdvanced()
        {
            var ns = TrakHoundPath.GetNamespace(Data.Path);
            var partialPath = TrakHoundPath.GetPartialPath(Data.Path);

            var publishEntity = new TrakHoundObjectEntity(partialPath, Data.ContentType.ConvertEnum<TrakHoundObjectContentType>(), Data.DefinitionId, ns);

            if (await _explorerService.Client.System.Entities.Objects.Publish(publishEntity, TrakHoundOperationMode.Sync))
            {
                var entity = await _explorerService.Client.System.Entities.Objects.ReadByUuid(publishEntity.Uuid);
                if (entity != null)
                {
                    await _explorerService.AddObject(entity);
                    _explorerService.SelectObject(entity.Uuid);

                    _explorerService.AddNotification(NotificationType.Information, "Object Added Successfully", entity.GetAbsolutePath());
                }
                else
                {
                    _explorerService.AddNotification(NotificationType.Error, "Error Reading Entity", entity.Uuid);
                }
            }
            else
            {
                _explorerService.AddNotification(NotificationType.Error, "Error Adding Entity");
            }
        }


        private async Task Edit()
        {
            var ns = TrakHoundPath.GetNamespace(Data.Path);
            var partialPath = TrakHoundPath.GetPartialPath(Data.Path);

            var publishEntity = new TrakHoundObjectEntity(partialPath, Data.ContentType.ConvertEnum<TrakHoundObjectContentType>(), Data.DefinitionId, ns);

            if (await _explorerService.Client.System.Entities.Objects.Publish(publishEntity, TrakHoundOperationMode.Sync))
            {
                var entity = await _explorerService.Client.System.Entities.Objects.ReadByUuid(publishEntity.Uuid);
                if (entity != null)
                {
                    await _explorerService.AddObject(entity);
                    _explorerService.SelectObject(entity.Uuid);

                    _explorerService.AddNotification(NotificationType.Information, "Object Added Successfully", entity.GetAbsolutePath());
                }
                else
                {
                    _explorerService.AddNotification(NotificationType.Error, "Error Reading Entity", entity.Uuid);
                }
            }
            else
            {
                _explorerService.AddNotification(NotificationType.Error, "Error Saving Entity");
            }
        }
    }
}
