using AutoMapper;
using Core.Models;
using TrackingSystemWeb.ViewModel;

namespace TrackingSystemWeb.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<LocationModel, LocationsViewModel>();
        }
    }
}