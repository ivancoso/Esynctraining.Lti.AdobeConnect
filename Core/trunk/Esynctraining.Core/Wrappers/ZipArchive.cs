namespace Esynctraining.Core.Wrappers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The zip archive.
    /// </summary>
    public class ZipArchive : IDisposable
    {
        #region Fields

        /// <summary>
        /// The external.
        /// </summary>
        private object external;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="ZipArchive"/> class from being created.
        /// </summary>
        private ZipArchive()
        {
        }

        #endregion

        #region Enums

        /// <summary>
        /// The compression method enumeration.
        /// </summary>
        public enum CompressionMethodEnum
        {
            /// <summary>
            /// The stored.
            /// </summary>
            Stored, 

            /// <summary>
            /// The deflated.
            /// </summary>
            Deflated
        }

        /// <summary>
        /// The deflate option enumeration.
        /// </summary>
        public enum DeflateOptionEnum
        {
            /// <summary>
            /// The normal.
            /// </summary>
            Normal, 

            /// <summary>
            /// The maximum.
            /// </summary>
            Maximum, 

            /// <summary>
            /// The fast.
            /// </summary>
            Fast, 

            /// <summary>
            /// The super fast.
            /// </summary>
            SuperFast
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the file names.
        /// </summary>
        public IEnumerable<string> FileNames
        {
            get
            {
                return this.Files.Select(p => p.Name).OrderBy(p => p);
            }
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        public IEnumerable<ZipFileInfo> Files
        {
            get
            {
                MethodInfo meth = this.external.GetType().GetMethod("GetFiles", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var zipFileCollection = meth.Invoke(this.external, null) as IEnumerable; // ZipFileInfoCollection
                return from object part in zipFileCollection select new ZipFileInfo { external = part };
            }
        }

        #endregion

        // ...
        #region Public Methods and Operators

        /// <summary>
        /// The open on file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <param name="share">
        /// The share.
        /// </param>
        /// <param name="streaming">
        /// The streaming.
        /// </param>
        /// <returns>
        /// The <see cref="ZipArchive"/>.
        /// </returns>
        public static ZipArchive OpenOnFile(
            string path, 
            FileMode mode = FileMode.Open, 
            FileAccess access = FileAccess.Read, 
            FileShare share = FileShare.Read, 
            bool streaming = false)
        {
            Type type = typeof(Package).Assembly.GetType("MS.Internal.IO.Zip.ZipArchive");
            MethodInfo meth = type.GetMethod(
                "OpenOnFile", 
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return new ZipArchive
            {
                external = meth.Invoke(null, new object[] { path, mode, access, share, streaming })
            };
        }

        /// <summary>
        /// The open on stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <param name="streaming">
        /// The streaming.
        /// </param>
        /// <returns>
        /// The <see cref="ZipArchive"/>.
        /// </returns>
        public static ZipArchive OpenOnStream(
            Stream stream, 
            FileMode mode = FileMode.OpenOrCreate, 
            FileAccess access = FileAccess.ReadWrite, 
            bool streaming = false)
        {
            Type type = typeof(Package).Assembly.GetType("MS.Internal.IO.Zip.ZipArchive");
            MethodInfo meth = type.GetMethod(
                "OpenOnStream", 
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return new ZipArchive { external = meth.Invoke(null, new object[] { stream, mode, access, streaming }) };
        }

        /// <summary>
        /// The add file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="compmeth">
        /// The compression method.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="ZipFileInfo"/>.
        /// </returns>
        public ZipFileInfo AddFile(string path, CompressionMethodEnum compmeth = CompressionMethodEnum.Deflated, DeflateOptionEnum option = DeflateOptionEnum.Normal)
        {
            Type type = this.external.GetType();
            MethodInfo meth = type.GetMethod(
                "AddFile", 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            object comp =
                type.Assembly.GetType("MS.Internal.IO.Zip.CompressionMethodEnum")
                    .GetField(compmeth.ToString())
                    .GetValue(null);
            object opti =
                type.Assembly.GetType("MS.Internal.IO.Zip.DeflateOptionEnum").GetField(option.ToString()).GetValue(null);
            return new ZipFileInfo { external = meth.Invoke(this.external, new[] { path, comp, opti }) };
        }

        /// <summary>
        /// The delete file.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void DeleteFile(string name)
        {
            MethodInfo meth = this.external.GetType()
                .GetMethod("DeleteFile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            meth.Invoke(this.external, new object[] { name });
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)this.external).Dispose();
        }

        /// <summary>
        /// The get file.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="ZipFileInfo"/>.
        /// </returns>
        public ZipFileInfo GetFile(string name)
        {
            MethodInfo meth = this.external.GetType()
                .GetMethod("GetFile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return new ZipFileInfo { external = meth.Invoke(this.external, new object[] { name }) };
        }

        #endregion

        /// <summary>
        /// The zip file info.
        /// </summary>
        public struct ZipFileInfo
        {
            #region Fields

            /// <summary>
            /// The external.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            internal object external;

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets the compression method.
            /// </summary>
            public object CompressionMethod
            {
                get
                {
                    return this.GetProperty("CompressionMethod");
                }
            }

            /// <summary>
            /// Gets the deflate option.
            /// </summary>
            public object DeflateOption
            {
                get
                {
                    return this.GetProperty("DeflateOption");
                }
            }

            /// <summary>
            /// Gets a value indicating whether folder flag.
            /// </summary>
            public bool FolderFlag
            {
                get
                {
                    return (bool)this.GetProperty("FolderFlag");
                }
            }

            /// <summary>
            /// Gets the last mod file date time.
            /// </summary>
            public DateTime LastModFileDateTime
            {
                get
                {
                    return (DateTime)this.GetProperty("LastModFileDateTime");
                }
            }

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name
            {
                get
                {
                    return (string)this.GetProperty("Name");
                }
            }

            /// <summary>
            /// Gets a value indicating whether volume label flag.
            /// </summary>
            public bool VolumeLabelFlag
            {
                get
                {
                    return (bool)this.GetProperty("VolumeLabelFlag");
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The get stream.
            /// </summary>
            /// <param name="mode">
            /// The mode.
            /// </param>
            /// <param name="access">
            /// The access.
            /// </param>
            /// <returns>
            /// The <see cref="Stream"/>.
            /// </returns>
            public Stream GetStream(FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
            {
                MethodInfo meth = this.external.GetType()
                    .GetMethod("GetStream", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return (Stream)meth.Invoke(this.external, new object[] { mode, access });
            }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return this.Name; // base.ToString();
            }

            #endregion

            #region Methods

            /// <summary>
            /// The get property.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <returns>
            /// The <see cref="object"/>.
            /// </returns>
            private object GetProperty(string name)
            {
                return
                    this.external.GetType()
                        .GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .GetValue(this.external, null);
            }

            #endregion
        }
    }
}