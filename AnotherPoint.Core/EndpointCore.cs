using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Entities.MethodImpl;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;

namespace AnotherPoint.Core
{
	public class EndpointCore : IEndpointCore
	{
		public string AppName { get; }

		public EndpointCore(string appName)
		{
			this.AppName = appName;
		}

		private Class entity;
		private Class common;

		public Endpoint ConstructEndpointFor(Class entityClass, bool includeEntityClass = true)
		{
			entityClass.Namespace = $"{this.AppName}.{Constant.Entities}";
			this.entity = entityClass;
			
			this.common = GetCommonClass();

			Endpoint endpoint = new Endpoint
			{
				CommonClass = this.common,
				EntityClass = this.entity,
				AppName = this.AppName,
			};

			endpoint.DAOInterfaces.AddRange(this.GetDAOInterfaces());
			endpoint.BLLInterfaces.AddRange(this.GetBLLInterfaces());
			endpoint.BLLClass = this.GetLogicClass(endpoint.BLLInterfaces, endpoint.DAOInterfaces.First(i => i.Name.EndsWith(Constant.DAO)));
			endpoint.DAOClass = this.GetDaoClass(endpoint.DAOInterfaces);

			return endpoint;
		}

		private Interface GetICrud(string a)
		{
			Interface iCrud = new Interface($"{this.AppName}.{a}.{Constant.Interfaces}.{Constant.ICrud}");

			iCrud.Namespace = $"{AppName}.{a}.{Constant.Interfaces}";

			iCrud.Usings.Add($"{AppName}.{Constant.Common}");
			iCrud.Usings.Add($"{AppName}.{Constant.Entities}");

			var createMeth = new Method("Create", Constant.Void) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			createMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName, BindSettings.None));

			var getMeth = new Method("Get", this.entity.FullName) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			getMeth.Arguments.Add(new Argument("id", Constant.Types.System_Guid, BindSettings.None));

			var removeMeth = new Method("Remove", Constant.Void) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			removeMeth.Arguments.Add(new Argument("id", Constant.Types.System_Guid, BindSettings.None));

			var updateMeth = new Method("Update", Constant.Void) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			updateMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName, BindSettings.None));

			iCrud.Methods.Add(createMeth);
			iCrud.Methods.Add(getMeth);
			iCrud.Methods.Add(removeMeth);
			iCrud.Methods.Add(updateMeth);

			return iCrud;
		}

		private Class GetCommonClass()
		{
			Class common = new Class($"{this.AppName}.{Constant.Common}");
			common.Namespace = $"{AppName}.{Constant.Common}";

			common.Usings.Add($"{Constant.Usings.System}");
			common.Constants.Add(new Field("ConnectionString", Constant.Types.System_String));

			return common;
		}

		private IEnumerable<Interface> GetBLLInterfaces()
		{
			Interface iCrud = GetICrud(Constant.BLL);

			Interface iEntityLogic = new Interface($"{this.AppName }.{Constant.BLL}.{Constant.Interfaces}.I{this.entity.Name}{Constant.Logic}");
			iEntityLogic.ImplementedInterfaces.Add(iCrud);

			IList<Interface> result = new List<Interface>();

			result.Add(iCrud);
			result.Add(iEntityLogic);

			return result;
		}

		private IEnumerable<Interface> GetDAOInterfaces()
		{
			Interface iCrud = GetICrud(Constant.BLL);

			Interface iEntityDao = new Interface($"{this.AppName }.{Constant.DAO}.{Constant.Interfaces}.I{this.entity.Name}{Constant.DAO}");
			iEntityDao.ImplementedInterfaces.Add(iCrud);

			IList<Interface> result = new List<Interface>();

			result.Add(iCrud);
			result.Add(iEntityDao);

			return result;
		}

		private Class GetDaoClass(IEnumerable<Interface> implementedInterfaces)
		{
			Class dao = new Class($"{AppName}.{Constant.DAO}.{this.entity.Name}{Constant.DAO}");

			foreach (var implementedInterface in implementedInterfaces)
			{
				dao.ImplementedInterfaces.Add(implementedInterface);

				if (!dao.Usings.Contains(implementedInterface.Namespace))
				{
					dao.Usings.Add(implementedInterface.Namespace);
				}
			}

			dao.Usings.Add(Constant.Usings.Dapper);
			dao.Usings.Add(Constant.Usings.System);
			dao.Usings.Add(Constant.Usings.System_Linq);
			dao.Usings.Add(this.common.Namespace);
			dao.Usings.Add(this.entity.Namespace);

			var createMeth = new Method("Create", Constant.Void) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			createMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName, BindSettings.None));
			createMeth.AttributesForBodyGeneration.Add(new MethodImpl.ToSqlAttribute());
			createMeth.AttributesForBodyGeneration.Add(new MethodImpl.ValidateAttribute(new[] { this.entity.Name.FirstLetterToLower() }));

			var getMeth = new Method("Get", this.entity.FullName) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			getMeth.Arguments.Add(new Argument("id", Constant.Types.System_Guid, BindSettings.None));
			createMeth.AttributesForBodyGeneration.Add(new MethodImpl.ToSqlAttribute());

			var removeMeth = new Method("Remove", Constant.Void) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			removeMeth.Arguments.Add(new Argument("id", Constant.Types.System_Guid, BindSettings.None));
			createMeth.AttributesForBodyGeneration.Add(new MethodImpl.ToSqlAttribute());

			var updateMeth = new Method("Update", Constant.Void) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			updateMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName, BindSettings.None));
			createMeth.AttributesForBodyGeneration.Add(new MethodImpl.ToSqlAttribute());
			updateMeth.AttributesForBodyGeneration.Add(new MethodImpl.ValidateAttribute(new[] { this.entity.Name.FirstLetterToLower() }));

			dao.Methods.Add(createMeth);
			dao.Methods.Add(getMeth);
			dao.Methods.Add(removeMeth);
			dao.Methods.Add(updateMeth);

			return dao;
		}

		private Class GetLogicClass(IEnumerable<Interface> implementedInterfaces, Interface destinationInterface)
		{
			Class bll = new Class($"{AppName}.{Constant.BLL}.{this.entity.Name}{Constant.Logic}");

			foreach (var implementedInterface in implementedInterfaces)
			{
				bll.ImplementedInterfaces.Add(implementedInterface);

				if (!bll.Usings.Contains(implementedInterface.Namespace))
				{
					bll.Usings.Add(implementedInterface.Namespace);
				}
			}

			bll.DestinationTypeName = destinationInterface.FullName;

			bll.Usings.Add(Constant.Usings.System);
			bll.Usings.Add(Constant.Usings.System_Linq);
			bll.Usings.Add(destinationInterface.Namespace);
			bll.Usings.Add(this.common.Namespace);
			bll.Usings.Add(this.entity.Namespace);

			var createMeth = new Method("Create", Constant.Void) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			createMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName, BindSettings.None));
			createMeth.AttributesForBodyGeneration.Add(new MethodImpl.SendMeToAttribute(Constant.DefaultDestination));
			createMeth.AttributesForBodyGeneration.Add(new MethodImpl.ValidateAttribute(new[] {this.entity.Name.FirstLetterToLower()}));

			var getMeth = new Method("Get", this.entity.FullName) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			getMeth.Arguments.Add(new Argument("id", Constant.Types.System_Guid, BindSettings.None));
			getMeth.AttributesForBodyGeneration.Add(new MethodImpl.SendMeToAttribute(Constant.DefaultDestination));

			var removeMeth = new Method("Remove", Constant.Void) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			removeMeth.Arguments.Add(new Argument("id", Constant.Types.System_Guid, BindSettings.None));
			removeMeth.AttributesForBodyGeneration.Add(new MethodImpl.SendMeToAttribute(Constant.DefaultDestination));

			var updateMeth = new Method("Update", Constant.Void) { AccessModifyer = AccessModifyer.Abstract | AccessModifyer.Virtual };
			updateMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName, BindSettings.None));
			updateMeth.AttributesForBodyGeneration.Add(new MethodImpl.SendMeToAttribute(Constant.DefaultDestination));
			updateMeth.AttributesForBodyGeneration.Add(new MethodImpl.ValidateAttribute(new[] { this.entity.Name.FirstLetterToLower() }));

			bll.Methods.Add(createMeth);
			bll.Methods.Add(getMeth);
			bll.Methods.Add(removeMeth);
			bll.Methods.Add(updateMeth);

			return bll;
		}
	}
}
