using AutoMapper;

namespace Server {

	public static class MapperConfig {

		public static void Load() {

			// outgoing
			Mapper.CreateMap<Persistence.Application, Application>();
			Mapper.CreateMap<Persistence.Package, Package>().ForMember(d => d.Version, o => o.MapFrom(s => s.Version.ToString(4)));

		}

	}

}