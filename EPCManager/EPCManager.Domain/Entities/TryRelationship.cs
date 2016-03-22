using EPCManager.Domain.Abstract.Entities;
using System;

namespace EPCManager.Domain.Entities
{
    public class TryRelationship : SPEntity
    {
        private IAssociableEntity _leftObject = new AssociableEntity();
        private IAssociableEntity _rightObject = new AssociableEntity();

        public virtual long OId { get; set; }
        public virtual SPObjectTypes LeftObjectType { get; set; }
        public virtual SPObjectTypes RightObjectType { get; set; }
        public virtual SPRelationshipType RelType { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual SPPeople CreatedBy { get; set; }
        public virtual bool IsReverse { get; set; }

        #region Embeded AssociableEntities

        public virtual IAssociableEntity LeftObject
        {
            get { return _leftObject; }
            set { _leftObject = value; }
        }

        public virtual IAssociableEntity RightObject
        {
            get { return _rightObject; }
            set { _rightObject = value; }
        }

        #endregion

        #region IAssociableEntity LeftObject

        public virtual long LeftObjectOId
        {
            get { return _leftObject.OId; }
            set { _leftObject.OId = value; }
        }

        public virtual string LeftRevision
        {
            get { return _leftObject.Revision; }
            set { _leftObject.Revision = value; }
        }

        public virtual SPClass LeftClass
        {
            get { return _leftObject.Class; }
            set { _leftObject.Class = value; }
        }

        public virtual string LeftDescription
        {
            get { return _leftObject.Description; }
            set { _leftObject.Description = value; }
        }

        public virtual SPStatus LeftStatus
        {
            get { return _leftObject.Status; }
            set { _leftObject.Status = value; }
        }

        public virtual SPIdentifier LeftIdentifier
        {
            get { return _leftObject.Identifier; }
            set { _leftObject.Identifier = value; }
        }

        #endregion

        #region IAssociableEntity RightObject

        public virtual long RightObjectOId
        {
            get { return _rightObject.OId; }
            set { _rightObject.OId = value; }
        }

        public virtual string RightRevision
        {
            get { return _rightObject.Revision; }
            set { _rightObject.Revision = value; }
        }

        public virtual SPClass RightClass
        {
            get { return _rightObject.Class; }
            set { _rightObject.Class = value; }
        }

        public virtual string RightDescription
        {
            get { return _rightObject.Description; }
            set { _rightObject.Description = value; }
        }

        public virtual SPStatus RightStatus
        {
            get { return _rightObject.Status; }
            set { _rightObject.Status = value; }
        }

        public virtual SPIdentifier RightIdentifier
        {
            get { return _rightObject.Identifier; }
            set { _rightObject.Identifier = value; }
        }

        #endregion
    }
}
