using System;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DockerPowerTools.Docker;
using DockerPowerTools.Registry;
using DockerPowerTools.RegistryCopy.Extensions;
using DockerPowerTools.RegistryCopy.Model;
using GalaSoft.MvvmLight;

namespace DockerPowerTools.RegistryCopy.ViewModel
{
    public class TagViewModel : ViewModelBase
    {
        private readonly TagModel _sourceModel;
        private string _target;
        private string _status;
        private string _targetRepository;

        public TagViewModel(TagModel sourceModel)
        {
            _sourceModel = sourceModel ?? throw new ArgumentNullException(nameof(sourceModel));

            TargetRepository = sourceModel.Repository;
            TargetTag = sourceModel.Tag;
        }

        public string Source => $"{_sourceModel.Repository}:{_sourceModel.Tag}";

        public string Target
        {
            get => _target;
            set
            {
                _target = value; 
                RaisePropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            private set
            {
                _status = value; 
                RaisePropertyChanged();
            }
        }

        public string TargetRepository
        {
            get => _targetRepository;
            set
            {
                _targetRepository = value; 
                RaisePropertyChanged();
            }
        }

        public string TargetTag { get; set; }

        public async Task CopyAsync(RegistryConnection sourceConnection, DockerConnection dockerConnection, RegistryConnection targetConnection, CancellationToken cancellationToken)
        {
            try
            {
                Status = "Pulling to Docker instance...";

                string fromImage = $"{_sourceModel.Registry}/{_sourceModel.Repository}:{_sourceModel.Tag}";

                var parameters = new ImagesCreateParameters
                {
                    FromImage = fromImage
                };

                var pullProgress = new Progress();

                //Do the pull
                await dockerConnection.DockerClient.Images.CreateImageAsync(
                    parameters, 
                    sourceConnection.GetAuthConfig(), 
                    pullProgress, 
                    cancellationToken);

                if (pullProgress.ErrorCount > 0)
                    throw new Exception(pullProgress.LastError);

                Status = "Tagging for target registry...";

                var imageTagParameters = new ImageTagParameters
                {
                    RepositoryName = $"{targetConnection.Registry}/{TargetRepository}",
                    Tag = TargetTag
                };

                // Tag it
                await dockerConnection.DockerClient.Images.TagImageAsync(fromImage, imageTagParameters, cancellationToken);

                Status = "Pushing to target registry...";

                var imagePushParameters = new ImagePushParameters
                {
                    ImageID = pullProgress.LastId,
                    RegistryAuth = targetConnection.GetAuthConfig(),
                    Tag = TargetTag,
                };

                //string toImage = 
                var pushProgress = new Progress();

                string toImage = $"{targetConnection.Registry}/{TargetRepository}:{TargetTag}";

                await dockerConnection.DockerClient.Images.PushImageAsync(toImage, imagePushParameters, targetConnection.GetAuthConfig(), pushProgress, cancellationToken);

                if (pushProgress.ErrorCount > 0)
                    throw new Exception(pushProgress.LastError);

                Status = "Done";
            }
            catch (Exception ex)
            {
                Status = ex.Message;
            }
        }

        
    }

    public class Progress : IProgress<JSONMessage>
    {
        public string LastError { get; private set; }

        public int ErrorCount { get; private set; }

        public string LastId { get; private set; }

        public void Report(JSONMessage value)
        {
            if (!string.IsNullOrWhiteSpace(value.ErrorMessage))
            {
                LastError = value.ErrorMessage;
                ErrorCount++;
            }

            if (!string.IsNullOrWhiteSpace(value.ID))
            {
                LastId = value.ID;
            }
        }
    }

    
}