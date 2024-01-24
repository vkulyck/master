using AutoMapper;
using AutoMapper.Configuration;
using GmWeb.Logic.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GmWeb.Logic.Utility.Mapping
{
    public class MappingFactory : IDisposableMapper
    {
        private MapperConfigurationExpression Config { get; set; }

        private IMapper _mapper;
        private IMapper Mapper
        {
            get
            {
                if (this.Dirty || this._mapper == null)
                {
                    this.Initialize();
                }
                return this._mapper;
            }
            set => this._mapper = value;
        }
        private bool Dirty { get; set; } = true;

        public IConfigurationProvider ConfigurationProvider => this.Mapper?.ConfigurationProvider;

        public Func<Type, object> ServiceCtor => this.Mapper?.ServiceCtor;

        public MappingFactory()
        {
            this.Config = new MapperConfigurationExpression();
        }

        public MappingFactory AddProfile<TProfile>()
            where TProfile : Profile, new()
        {
            this.Dirty = true;
            this.Config.AddProfile<TProfile>();
            return this;
        }

        private MappingFactory Initialize()
        {
            var config = new MapperConfiguration(this.Config);
            this.Mapper = config.CreateMapper();
            this.Dirty = false;
            return this;
        }

        #region Collection Mapping

        public IEnumerable<T> Map<T>(IEnumerable collection)
        {
            foreach (var model in collection)
            {
                var mapped = this.Map<T>(model);
                yield return mapped;
            }
        }
        public IEnumerable<U> Map<T, U>(IEnumerable<T> collection)
        {
            foreach (var model in collection)
            {
                var mapped = this.Map<T, U>(model);
                yield return mapped;
            }
        }

        public MapInferrenceWrapper<TSource> Map<TSource>(IEnumerable<TSource> source)
        {
            return new MapInferrenceWrapper<TSource>(this, source);
        }
        public MapInferrenceWrapper<TSource> Map<TSource>(IQueryable<TSource> source)
        {
            return new MapInferrenceWrapper<TSource>(this, source);
        }

        #endregion

        #region Cloning
        public U Clone<T, U>(T inputModel)
            where U : class, new()
        {
            var outputModel = new U();
            this.Map(inputModel, outputModel);
            return outputModel;
        }

        public T Clone<T>(object inputModel)
            where T : class, new()
            => this.Clone<object, T>(inputModel)
        ;

        public T Clone<T>(T inputModel)
            where T : class, new()
            => this.Map<T>(inputModel)
        ;
        #endregion

        #region IMapper Interface

        public TDestination Map<TDestination>(object source)
            => this.Mapper.Map<TDestination>(source);

        public TDestination Map<TSource, TDestination>(TSource source)
            => this.Mapper.Map<TSource, TDestination>(source);

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
            => this.Mapper.Map(source, destination);

        public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> opts)
            => this.Mapper.Map<TDestination>(source, opts);

        public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts)
            => this.Mapper.Map(source, opts);

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts)
            => this.Mapper.Map(source, destination, opts);

        public object Map(object source, Type sourceType, Type destinationType)
        => this.Mapper.Map(source, sourceType, destinationType);

        public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
            => this.Mapper.Map(source, sourceType, destinationType, opts);

        public object Map(object source, object destination, Type sourceType, Type destinationType)
            => this.Mapper.Map(source, destination, sourceType, destinationType);

        public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
            => this.Mapper.Map(source, destination, sourceType, destinationType, opts);

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, object parameters = null, params Expression<Func<TDestination, object>>[] membersToExpand)
            => this.Mapper.ProjectTo(source, parameters, membersToExpand);

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, IDictionary<string, object> parameters, params string[] membersToExpand)
            => this.Mapper.ProjectTo<TDestination>(source, parameters, membersToExpand);

        #endregion

        public void Dispose() { }
    }
}