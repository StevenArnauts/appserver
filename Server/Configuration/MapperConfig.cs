using AutoMapper;
using Core.Persistence;

namespace Server {

	public static class MapperConfig {

		public static void Load() {

			// outgoing
			Mapper.CreateMap<FileSystemApplication, Application>();
			Mapper.CreateMap<FileSystemPackage, Package>().ForMember(d => d.Version, o => o.MapFrom(s => s.Version.ToString(4)));

		}

	}

}