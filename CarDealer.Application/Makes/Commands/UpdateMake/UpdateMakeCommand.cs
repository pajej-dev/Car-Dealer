﻿using CarDealer.Application.Interfaces.CQRS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.Application.Makes.Commands.UpdateMake
{
	public class UpdateMakeCommand : IRequest
	{
		public int Id { get; }
		public string Name { get; }

		public UpdateMakeCommand(int id, string name)
		{
			this.Id = id;
			this.Name = name;
		}

	}
}
