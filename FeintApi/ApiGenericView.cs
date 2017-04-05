using System;
using FeintSDK;
using FeintSDK.Exceptions;
using System.Linq;
using Newtonsoft.Json;
using static FeintSDK.Helpers;

namespace FeintApi
{
    public abstract class ApiGenericView<T> : ClassView where T : BaseModel
    {

        protected IQueryable<T> queryable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected T dataObject
        {
            get
            {
                var id = int.Parse(this.request.Variables["id"].Value);
                return GetObjectOr404(queryable.Where(x => x.Id == id));
            }
        }

        protected Type serializer
        {
            get
            {
                return typeof(T);
            }
        }

        protected Response retrieve(Request request)
        {
            return new ApiResponse(dataObject);
        }

        protected Response list(Request request)
        {
            return new ApiResponse(queryable);
        }
        protected Response create(Request request)
        {
            IModelWritable instance = (IModelWritable) JsonConvert.DeserializeObject(request.Body,serializer);
            instance.Save();
            return new ApiResponse(instance) { Status = 201 };
        }

        protected Response update(Request request, bool partial = false)
        {
            if (partial)
                throw new NotImplementedException();
            var instance = this.dataObject;
            if (typeof(T) == serializer)
            {
                var updated = (T)JsonConvert.DeserializeObject(request.Body, serializer);
                updated.Id = instance.Id;
                instance = updated;
                instance.Save();
            }
            return new ApiResponse(instance);
        }

        protected Response destroy(Request request)
        {
            dataObject.delete();
            return new ApiResponse(204);
        }

        protected Response get(Request request)
        {
            if (this is IRetrieveApiView)
                return this.retrieve(request);
            if (this is IListApiView)
                return this.list(request);
            throw new Http405Exception();
        }
        protected Response post(Request request)
        {
            if(this is ICreateApiView)
                return this.create(request);
            throw new Http405Exception();
        }

        protected Response put(Request request)
        {
            if(this is IUpdateApiView)
                return this.update(request);
            throw new Http405Exception();
        }

        protected Response patch(Request request)
        {
            if (this is IUpdateApiView)
                return this.update(request, partial: true);
            throw new Http405Exception();
        }

        protected Response delete(Request request)
        {
            if(this is IDestroyApiView)
                return this.destroy(request);
            throw new Http405Exception();
        }

        protected override Response processRequest()
        {
            switch (request.Method)
            {
                case RequestMethod.GET:
                    return this.get(request);
                case RequestMethod.POST:
                    return this.post(request);
                case RequestMethod.PUT:
                    return this.put(request);
                case RequestMethod.PATCH:
                    return this.patch(request);
                case RequestMethod.DELETE:
                    return this.delete(request);
            }
            throw new Http405Exception();
        }

        public ApiGenericView(Request request) : base(request)
        {
        }

    }
    public interface ICreateApiView { };
    public interface IRetrieveApiView { };
    public interface IUpdateApiView { };
    public interface IDestroyApiView { };
    public interface IListApiView { };


    public interface IFullApiView : ICreateApiView, IRetrieveApiView, IUpdateApiView, IDestroyApiView, IListApiView { };
    public interface IListCreateApiView : IListApiView, ICreateApiView { };
    public interface IRetrieveUpdateApiView : IListApiView, ICreateApiView { };
    public interface IRetrieveUpdateDestroyApiView : IListApiView, ICreateApiView, IDestroyApiView { };

}

