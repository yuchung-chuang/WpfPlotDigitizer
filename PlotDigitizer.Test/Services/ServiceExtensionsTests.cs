using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace PlotDigitizer.Core.Tests
{
    [TestClass]
    public class ServiceExtensionsTests
    {
        // ---------------------------------------------------------------
        // AddViewModels
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void AddViewModels_MainViewModel_RegisteredAsTransient()
        {
            var sc = new ServiceCollection();
            sc.AddViewModels();
            Assert.IsTrue(HasDescriptor(sc, typeof(MainViewModel), ServiceLifetime.Transient));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddViewModels_LoadPageViewModel_RegisteredAsTransient()
        {
            var sc = new ServiceCollection();
            sc.AddViewModels();
            Assert.IsTrue(HasDescriptor(sc, typeof(LoadPageViewModel), ServiceLifetime.Transient));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddViewModels_RangePageViewModel_RegisteredAsTransient()
        {
            var sc = new ServiceCollection();
            sc.AddViewModels();
            Assert.IsTrue(HasDescriptor(sc, typeof(RangePageViewModel), ServiceLifetime.Transient));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddViewModels_AxisPageViewModel_RegisteredAsTransient()
        {
            var sc = new ServiceCollection();
            sc.AddViewModels();
            Assert.IsTrue(HasDescriptor(sc, typeof(AxisPageViewModel), ServiceLifetime.Transient));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddViewModels_FilterPageViewModel_RegisteredAsTransient()
        {
            var sc = new ServiceCollection();
            sc.AddViewModels();
            Assert.IsTrue(HasDescriptor(sc, typeof(FilterPageViewModel), ServiceLifetime.Transient));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddViewModels_DataPageViewModel_RegisteredAsTransient()
        {
            var sc = new ServiceCollection();
            sc.AddViewModels();
            Assert.IsTrue(HasDescriptor(sc, typeof(DataPageViewModel), ServiceLifetime.Transient));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddViewModels_EditPageViewModel_RegisteredAsScoped()
        {
            var sc = new ServiceCollection();
            sc.AddViewModels();
            Assert.IsTrue(HasDescriptor(sc, typeof(EditPageViewModel), ServiceLifetime.Scoped));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddViewModels_IEditServiceOfImage_RegisteredAsScoped()
        {
            var sc = new ServiceCollection();
            sc.AddViewModels();
            Assert.IsTrue(HasDescriptor(sc, typeof(IEditService<Image<Rgba, byte>>), ServiceLifetime.Scoped));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddViewModels_IEditServiceOfImage_ImplementedByEditService()
        {
            var sc = new ServiceCollection();
            sc.AddViewModels();
            var descriptor = sc.FirstOrDefault(d => d.ServiceType == typeof(IEditService<Image<Rgba, byte>>));
            Assert.IsNotNull(descriptor);
            Assert.AreEqual(typeof(EditService<Image<Rgba, byte>>), descriptor.ImplementationType);
        }

        // ---------------------------------------------------------------
        // AddModel
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_Model_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(Model), ServiceLifetime.Singleton));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_Model_ImplementedByUpdatableModel()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            var descriptor = sc.FirstOrDefault(d => d.ServiceType == typeof(Model));
            Assert.IsNotNull(descriptor);
            Assert.AreEqual(typeof(UpdatableModel), descriptor.ImplementationType);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_Setting_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(Setting), ServiceLifetime.Singleton));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_Setting_ImplementedByUpdatableSetting()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            var descriptor = sc.FirstOrDefault(d => d.ServiceType == typeof(Setting));
            Assert.IsNotNull(descriptor);
            Assert.AreEqual(typeof(UpdatableSetting), descriptor.ImplementationType);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_InputImageNode_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(InputImageNode), ServiceLifetime.Singleton));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_CroppedImageNode_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(CroppedImageNode), ServiceLifetime.Singleton));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_FilteredImageNode_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(FilteredImageNode), ServiceLifetime.Singleton));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_EdittedImageNode_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(EdittedImageNode), ServiceLifetime.Singleton));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_DataPointsNode_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(DataPointsNode), ServiceLifetime.Singleton));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_DataNode_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(DataNode), ServiceLifetime.Singleton));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_AxisLocationNode_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(AxisLocationNode), ServiceLifetime.Singleton));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AddModel_DataTypeNode_RegisteredAsSingleton()
        {
            var sc = new ServiceCollection();
            sc.AddModel();
            Assert.IsTrue(HasDescriptor(sc, typeof(DataTypeNode), ServiceLifetime.Singleton));
        }

        // ---------------------------------------------------------------
        // Helper
        // ---------------------------------------------------------------

        private static bool HasDescriptor(IServiceCollection sc, System.Type serviceType, ServiceLifetime lifetime)
            => sc.Any(d => d.ServiceType == serviceType && d.Lifetime == lifetime);
    }
}
