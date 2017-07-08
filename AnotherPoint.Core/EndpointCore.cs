using AnotherPoint.Common;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Entities.MethodImpl;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AnotherPoint.Core
{
	public class EndpointCore : IEndpointCore
	{
		private Class common;
		private Class entity;

		public EndpointCore(string appName)
		{
			this.AppName = appName;
		}

		public string AppName { get; }

		public Endpoint ConstructEndpointFor(Class entityClass)
		{
			Log.Info($"Constructing endpoint for {entityClass.FullName}...");

			Stopwatch sw = Stopwatch.StartNew();

			// Have I to rewrite the namespace for true one? TODO ot TOTHINK
			entityClass.Namespace = $"{this.AppName}.{Constant.Entities}";

			this.entity = entityClass;
			this.common = this.GetCommonClass();

			Endpoint endpoint = new Endpoint
			{
				CommonClass = this.common,
				EntityClass = this.entity,
				AppName = this.AppName,
			};

			endpoint.DaoInterfaces.AddRange(this.GetDaoInterfaces());
			endpoint.BllInterfaces.AddRange(this.GetBllInterfaces());
			endpoint.BllClass = this.GetLogicClass(endpoint.BllInterfaces, endpoint.DaoInterfaces.First(i => Helpers.NameWithoutGeneric(i.FullName).EndsWith(Constant.Dao)));
			endpoint.DaoClass = this.GetDaoClass(endpoint.DaoInterfaces);
			endpoint.BllClass.Validation = RenderEngine.ValidationCore.ConstructValidationClass($"{endpoint.AppName}.{Constant.Bll}");
			endpoint.DaoClass.Validation = RenderEngine.ValidationCore.ConstructValidationClass($"{endpoint.AppName}.{Constant.Dao}");

			sw.Stop();

			Log.iDone(sw.Elapsed.TotalMilliseconds);

			return endpoint;
		}

		private IEnumerable<Interface> GetBllInterfaces()
		{
			Interface iCrud = this.GetICrud(Constant.Bll);
			Interface iEntityLogic = this.GetIEntityLogic(iCrud);
			iEntityLogic.OverrideGenericTypes.Add("T", this.entity.FullName);

			IList<Interface> result = new List<Interface>();

			result.Add(iCrud);
			result.Add(iEntityLogic);

			return result;
		}

		private Class GetCommonClass()
		{
			Class common = new Class($"{this.AppName}.{Constant._Constant}")
			{
				Namespace = $"{this.AppName}.{Constant.Common}"
			};

			common.Usings.Add($"{Constant.Usings.System}");
			common.Constants.Add(new Field(Constant.ConnectionString, Constant.Types.SystemString));

			return common;
		}

		private Class GetDaoClass(IEnumerable<Interface> implementedInterfaces)
		{
			Class dao = new Class($"{this.AppName}.{Constant.Dao}.{this.entity.Name}{Constant.Dao}");

			this.SetupDaoUsings(dao);
			this.SetupDaoInterfaces(implementedInterfaces, dao);
			this.SetupDaoReferences(dao);
			this.SetupDaoPackages(dao);
			this.SetupDaoMethods(dao);

			dao.OverrideGenericTypes.Add("T", this.entity.FullName);

			return dao;
		}

		private IEnumerable<Interface> GetDaoInterfaces()
		{
			Interface iCrud = this.GetICrud(Constant.Dao);
			Interface iEntityDao = this.GetIEntityDao(iCrud);
			iEntityDao.OverrideGenericTypes.Add("T", this.entity.FullName);

			IList<Interface> result = new List<Interface>();

			result.Add(iCrud);
			result.Add(iEntityDao);

			return result;
		}

		private Interface GetICrud(string a)
		{
			Interface iCrud = new Interface($"{this.AppName}.{a}.{Constant.Interfaces}.{Constant.ICrud}<T>")
			{
				AccessModifyer = AccessModifyer.Public,
				Namespace = $"{this.AppName}.{a}.{Constant.Interfaces}",
			};

			iCrud.Type.IsGeneric = true;
			iCrud.Type.GenericTypes.Add("T");

			this.SetupICrudUsings(iCrud);
			this.SetupICrudMethods(iCrud);

			return iCrud;
		}

		private Interface GetIEntityDao(Interface implementInterface) => this.GetIEntityDao(new[] { implementInterface });

		private Interface GetIEntityDao(IEnumerable<Interface> implementInterfaces)
		{
			Interface iEntityDao = new Interface($"{this.AppName}.{Constant.Dao}.{Constant.Interfaces}.I{this.entity.Name}{Constant.Dao}<{this.entity.FullName}>")
			{
				AccessModifyer = AccessModifyer.Public
			};

			foreach (var implementInterface in implementInterfaces)
			{
				iEntityDao.ImplementedInterfaces.Add(implementInterface);
			}

			return iEntityDao;
		}

		private Interface GetIEntityLogic(Interface implementedInterface) => this.GetIEntityLogic(new[] { implementedInterface });

		private Interface GetIEntityLogic(IEnumerable<Interface> implementedInterfaces)
		{
			Interface iEntityLogic = new Interface($"{this.AppName}.{Constant.Bll}.{Constant.Interfaces}.I{this.entity.Name}{Constant.Logic}<{this.entity.FullName}>")
			{
				AccessModifyer = AccessModifyer.Public
			};

			foreach (var implementedInterface in implementedInterfaces)
			{
				iEntityLogic.ImplementedInterfaces.Add(implementedInterface);
			}

			return iEntityLogic;
		}

		private Class GetLogicClass(IEnumerable<Interface> implementedInterfaces, Interface destinationInterface)
		{
			Class bll = new Class($"{this.AppName}.{Constant.Bll}.{this.entity.Name}{Constant.Logic}");

			this.SetupLogicInterfaces(implementedInterfaces, bll);
			this.SetupLogicEndpoint(destinationInterface, bll);
			this.SetupLogicUsings(destinationInterface, bll);
			this.SetupLogicMethods(bll);

			bll.OverrideGenericTypes.Add("T", this.entity.FullName);

			return bll;
		}

		private void SetupDaoInterfaces(IEnumerable<Interface> implementedInterfaces, Class dao)
		{
			foreach (var implementedInterface in implementedInterfaces)
			{
				dao.ImplementedInterfaces.Add(implementedInterface);

				if (!dao.Usings.Contains(implementedInterface.Namespace))
				{
					dao.Usings.Add(implementedInterface.Namespace);
				}
			}
		}

		private void SetupDaoMethods(Class dao)
		{
			// The point of this formatting is to almost-split parts to methods. They are too small for methods, too different for a unique helper, but too big to have all them in one place.

			{
				var createMeth = new Method("Create", Constant.Types.SystemVoid);
				createMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName,
					BindSettings.None));
				createMeth.AttributesForBodyGeneration.Add(new MethodImpl.ToSqlAttribute());
				createMeth.AttributesForBodyGeneration.Add(
					new MethodImpl.ValidateAttribute(new[] { this.entity.Name.FirstLetterToLower() }));
				dao.Methods.Add(createMeth);
			}

			{
				var getMeth = new Method("Get", this.entity.FullName);
				getMeth.Arguments.Add(new Argument("id", Constant.Types.SystemGuid, BindSettings.None));
				getMeth.AttributesForBodyGeneration.Add(new MethodImpl.ToSqlAttribute());
				dao.Methods.Add(getMeth);
			}

			{
				var removeMeth = new Method("Remove", Constant.Types.SystemVoid);

				removeMeth.Arguments.Add(new Argument("id", Constant.Types.SystemGuid, BindSettings.None));
				removeMeth.AttributesForBodyGeneration.Add(new MethodImpl.ToSqlAttribute());
				dao.Methods.Add(removeMeth);
			}

			{
				var updateMeth = new Method("Update", Constant.Types.SystemVoid);
				updateMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName,
					BindSettings.None));
				updateMeth.AttributesForBodyGeneration.Add(new MethodImpl.ToSqlAttribute());
				updateMeth.AttributesForBodyGeneration.Add(
					new MethodImpl.ValidateAttribute(new[] { this.entity.Name.FirstLetterToLower() }));
				dao.Methods.Add(updateMeth);
			}
		}

		private void SetupDaoPackages(Class dao)
		{
			dao.PackageAttributes.Add(NugetPackageRepository.Dapper);
		}

		private void SetupDaoReferences(Class dao)
		{
			dao.References.Add(Constant.Types.SystemData);
		}

		private void SetupDaoUsings(Class dao)
		{
			dao.Usings.Add(Constant.Usings.Dapper);
			dao.Usings.Add(Constant.Usings.System);
			dao.Usings.Add(Constant.Usings.SystemLinq);
			dao.Usings.Add(this.common.Namespace);
			dao.Usings.Add(this.entity.Namespace);
		}

		private void SetupICrudMethods(Interface iCrud)
		{
			// The point of this formatting is to almost-split parts to methods. They are too small for methods, too different for a unique helper, but too big to have all them in one place.
			{
				var createMeth = new Method("Create", Constant.Types.SystemVoid)
				{
					AccessModifyer = AccessModifyer.Abstract |
									 AccessModifyer.Virtual
				};
				createMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), "T",
					BindSettings.None));
				iCrud.Methods.Add(createMeth);
			}

			{
				var getMeth = new Method("Get", "T")
				{
					AccessModifyer = AccessModifyer.Abstract |
									 AccessModifyer.Virtual
				};
				getMeth.Arguments.Add(new Argument("id", Constant.Types.SystemGuid, BindSettings.None));
				iCrud.Methods.Add(getMeth);
			}

			{
				var removeMeth = new Method("Remove", Constant.Types.SystemVoid)
				{
					AccessModifyer = AccessModifyer.Abstract |
									 AccessModifyer.Virtual
				};
				removeMeth.Arguments.Add(new Argument("id", Constant.Types.SystemGuid, BindSettings.None));

				iCrud.Methods.Add(removeMeth);
			}

			{
				var updateMeth = new Method("Update", Constant.Types.SystemVoid)
				{
					AccessModifyer = AccessModifyer.Abstract |
									 AccessModifyer.Virtual
				};
				updateMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), "T",
					BindSettings.None));
				iCrud.Methods.Add(updateMeth);
			}
		}

		private void SetupICrudUsings(Interface iCrud)
		{
			iCrud.Usings.Add($"{this.AppName}.{Constant.Common}");
			iCrud.Usings.Add($"{this.AppName}.{Constant.Entities}");
		}

		private void SetupLogicEndpoint(Interface destinationInterface, Class bll)
		{
			bll.EntityPurposePair = new EntityPurposePair(this.entity.Type.Name, Constant.Logic);

			bll.DestinationTypeName = Helpers.NameWithoutGeneric(destinationInterface.FullName);
			bll.IsEndpoint = true;

			Field destinationField = RenderEngine.ClassCore.GetDestinationFieldForInject(bll);
			bll.Fields.Add(destinationField);

			var injectCtor = RenderEngine.ClassCore.GetInjectCtorForDestinationField(bll.FullName, destinationField);
			bll.Ctors.Add(injectCtor);
		}

		private void SetupLogicInterfaces(IEnumerable<Interface> implementedInterfaces, Class bll)
		{
			foreach (var implementedInterface in implementedInterfaces)
			{
				bll.ImplementedInterfaces.Add(implementedInterface);

				if (!bll.Usings.Contains(implementedInterface.Namespace))
				{
					bll.Usings.Add(implementedInterface.Namespace);
				}
			}
		}

		private void SetupLogicMethods(Class bll)
		{
			// The point of this formatting is to almost-split parts to methods. They are too small for methods, too different for a unique helper, but too big to have all them in one place.
			{
				var createMeth = new Method("Create", Constant.Types.SystemVoid);

				createMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName,
					BindSettings.None));
				createMeth.AttributesForBodyGeneration.Add(new MethodImpl.SendMeToAttribute(Constant.DefaultDestination, bll.Name));
				createMeth.AttributesForBodyGeneration.Add(
					new MethodImpl.ValidateAttribute(new[] { this.entity.Name.FirstLetterToLower() }));
				bll.Methods.Add(createMeth);
			}

			{
				var getMeth = new Method("Get", this.entity.FullName);
				getMeth.Arguments.Add(new Argument("id", Constant.Types.SystemGuid, BindSettings.None));
				getMeth.AttributesForBodyGeneration.Add(new MethodImpl.SendMeToAttribute(Constant.DefaultDestination, bll.Name));
				bll.Methods.Add(getMeth);
			}

			{
				var removeMeth = new Method("Remove", Constant.Types.SystemVoid);
				removeMeth.Arguments.Add(new Argument("id", Constant.Types.SystemGuid, BindSettings.None));
				removeMeth.AttributesForBodyGeneration.Add(new MethodImpl.SendMeToAttribute(Constant.DefaultDestination, bll.Name));
				bll.Methods.Add(removeMeth);
			}

			{
				var updateMeth = new Method("Update", Constant.Types.SystemVoid);

				updateMeth.Arguments.Add(new Argument(this.entity.Name.FirstLetterToLower(), this.entity.Type.FullName,
					BindSettings.None));
				updateMeth.AttributesForBodyGeneration.Add(new MethodImpl.SendMeToAttribute(Constant.DefaultDestination, bll.Name));
				updateMeth.AttributesForBodyGeneration.Add(
					new MethodImpl.ValidateAttribute(new[] { this.entity.Name.FirstLetterToLower() }));
				bll.Methods.Add(updateMeth);
			}
		}

		private void SetupLogicUsings(Interface destinationInterface, Class bll)
		{
			bll.Usings.Add(Constant.Usings.System);
			bll.Usings.Add(Constant.Usings.SystemLinq);
			bll.Usings.Add(destinationInterface.Namespace);
			bll.Usings.Add(this.common.Namespace);
			bll.Usings.Add(this.entity.Namespace);
		}
	}
}