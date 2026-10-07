using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Utilities
{
    public static class MapperProvider
    {
        public static IMapper Mapper { get; private set; }

        public static void Configure(IMapper mapper)
        {
            Mapper = mapper;
        }
    }

}
