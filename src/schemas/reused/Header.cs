namespace net.niceygy.eddatacollector.schemas.reused
{
        public class Header
    {
        public required string gamebuild { get; set; }
        /// <summary>
        /// From Fileheader event if available, else LoadGame if available there
        /// </summary>
        public required string gameversion { get; set; }
        /// <summary>
        /// Timestamp upon receipt at the gateway. If present, this property will be 
        /// overwritten by the gateway; submitters are not intended to populate this property.
        /// </summary>
        public DateTime gatewayTimestamp { get; set; }
        /// <summary>
        /// Identifier string for the uplaoder sofware.
        /// E.g "EDDI"
        /// </summary>
        public required string softwareName { get; set; }
        /// <summary>
        /// Version that the uploader is using.
        /// </summary>
        public required string softwareVersion { get; set; }
        /// <summary>
        /// Unique hash of uploader, changes once per 24H
        /// </summary>
        public required string uploaderID { get; set; }
    }
}