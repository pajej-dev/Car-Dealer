﻿using AutoMapper;
using CarDealer.Application.Interfaces;
using CarDealer.Application.Interfaces.CQRS;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarDealer.Application.Makes.Queries.GetMake
{
	public class GetMakeQueryHandler : IRequestHandler<GetMakeQuery, GetMakeDto>
	{
		private readonly ICarDealerContext context;
		private readonly IMapper mapper;

		public GetMakeQueryHandler(ICarDealerContext carDealerContext, IMapper mapper)
		{
			this.context = carDealerContext;
			this.mapper = mapper;
		}

		public async Task<GetMakeDto> Handle(GetMakeQuery request, CancellationToken cancellationToken)
		{
			var make = await this.context.Makes.FirstOrDefaultAsync(a => a.Id == request.Id);

			if (make == null)
				return null;

			return mapper.Map<GetMakeDto>(make);
		}

	}
}

