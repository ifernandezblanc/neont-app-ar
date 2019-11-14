﻿/*==============================================================================
Author: Iñigo Fernandez del Amo - 2019
Email: inigofernandezdelamo@outlook.com
License: This code has been developed for research and demonstration purposes.

Copyright (c) 2019 Iñigo Fernandez del Amo. All Rights Reserved.
Copyright (c) 2019 Cranfield University. All Rights Reserved.
Copyright (c) 2019 Babcock International Group. All Rights Reserved.

All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.

Date: 08/07/2019
==============================================================================*/

#region NAMESPACES
/// <summary>
/// Describe script purpose
/// Add links when code has been inspired
/// </summary>
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#endregion


namespace Rtrbau
{
    /// <summary>
    /// Describe script purpose
    /// Add links when code has been inspired
    /// </summary>
    #region DATA_CLASSES
    /// <summary>
    /// Describe script purpose
    /// Add links when code has been inspired
    /// </summary> 
    [Serializable]
    public class OntologyEntity
    {
        #region MEMBERS
        public string name;
        public string ontology;
        private string url;
        #endregion MEMBERS

        #region CONSTRUCTORS
        public OntologyEntity()
        {
            name = null;
            ontology = null;
            url = null;
        }

        public OntologyEntity(string entityURI)
        {
            string entity = Parser.ParseURI(entityURI, '/', RtrbauParser.post);
            name = Parser.ParseURI(entity, '#', RtrbauParser.post);
            ontology = Parser.ParseURI(entity, '#', RtrbauParser.pre);
            url = Parser.ParseURI(entityURI, '/', RtrbauParser.pre);
        }
        #endregion CONSTRUCTORS

        #region METHODS
        public string URI()
        {
            return url + "/" + Entity();
        }
        public string Entity()
        {
            return ontology + "#" + name;
        }

        public bool EqualEntity(OntologyEntity entity)
        {
            return name == entity.name && ontology == entity.ontology;
        }
        #endregion METHODS
    }

    /// <summary>
    /// Describe script purpose
    /// Add links when code has been inspired
    /// </summary> 
    [Serializable]
    public class OntologyElement : ILoadable
    {
        #region MEMBERS
        public OntologyEntity entity;
        public OntologyElementType type;
        #endregion MEMBERS

        #region CONSTRUCTOR
        public OntologyElement()
        {
            entity = new OntologyEntity();
            type = OntologyElementType.Ontologies;
        }

        public OntologyElement(string entityURI, OntologyElementType elementType)
        {
            entity = new OntologyEntity(entityURI);
            type = elementType;
        }
        #endregion CONSTRUCTOR

        #region ILOADABLE_METHODS
        public string URL()
        {
            return Parser.ParseOntElementURI(entity.name, entity.ontology, type);
        }
        public string FilePath()
        {
            string folder;

            if (Dictionaries.ontDataDirectories.TryGetValue(type, out folder)) { }
            else { throw new ArgumentException("Argument element error: ontology element type not implemented."); }

            return folder + '/' + entity.Entity() + ".json";
        }

        public string EventName()
        {
            // return type.ToString() + "_" + type.ToString() + "_" + entity.Entity();
            return "Ontology__" + type.ToString() + "__" + entity.Entity();
        }
        #endregion ILOADABLE_METHODS

        #region CLASS_METHODS
        public bool EqualElement(OntologyElement element)
        {
            return entity.EqualEntity(element.entity) && type.Equals(element.type);
        }
        #endregion CLASS_METHODS
    }

    /// <summary>
    /// Describe script purpose
    /// Add links when code has been inspired
    /// </summary> 
    [Serializable]
    public class OntologyUpload : ILoadable
    {
        #region MEMBERS
        public OntologyElement individualElement;
        public OntologyElement classElement;
        #endregion MEMBERS

        #region CONSTRUCTORS
        public OntologyUpload()
        {
            individualElement = new OntologyElement();
            classElement = new OntologyElement();
        }

        public OntologyUpload(OntologyElement elementIndividual, OntologyElement elementClass)
        {
            if (elementIndividual.entity.ontology == elementClass.entity.ontology)
            {
                individualElement = elementIndividual;
                classElement = elementClass;
            }
            else
            {
                throw new ArgumentException("OntologyData::OntologyUpload: individual and class must belong to the same ontology.");
            }
        }
        #endregion CONSTRUCTORS

        #region ILOADABLE_METHODS
        public string URL()
        {
            return Parser.ParseOntElementURI(individualElement.entity.name, individualElement.entity.ontology, OntologyElementType.IndividualUpload);
        }

        public string FilePath()
        {
            string folder;

            if (Dictionaries.ontDataDirectories.TryGetValue(OntologyElementType.IndividualUpload, out folder)) { }
            else { throw new ArgumentException("Argument element error: ontology element type not implemented."); }

            return folder + '/' + individualElement.entity.Entity() + ".json";
        }

        public string EventName()
        {
            return "Ontology__" + OntologyElementType.IndividualUpload + "__" + individualElement.entity.Entity();
        }
        #endregion ILOADABLE_METHODS
    }

    /// <summary>
    /// Describe script purpose
    /// Add links when code has been inspired
    /// </summary> 
    [Serializable]
    public class OntologyDistance : ILoadable
    {
        #region MEMBERS
        public OntologyEntity startClass;
        public OntologyEntity endClass;
        public RtrbauDistanceType distanceType;
        #endregion MEMBERS

        #region CONSTRUCTOR
        public OntologyDistance()
        {
            startClass = new OntologyEntity();
            endClass = new OntologyEntity();
            distanceType = RtrbauDistanceType.Component;
        }

        public OntologyDistance (string startClassURI, RtrbauDistanceType distance)
        {
            startClass = new OntologyEntity(startClassURI);

            if (distance == RtrbauDistanceType.Component)
            {
                if (Rtrbauer.instance.component.componentURI != null)
                {
                    endClass = new OntologyEntity(Rtrbauer.instance.component.componentURI);
                }
                else
                {
                    throw new ArgumentException("Argument distance error: component class not declared.");
                }
            }
            else if (distance == RtrbauDistanceType.Operation)
            {
                if (Rtrbauer.instance.operation.operationURI != null)
                {
                    endClass = new OntologyEntity(Rtrbauer.instance.operation.operationURI);
                }
                else
                {
                    throw new ArgumentException("Argument distance error: operation class not declared.");
                }
            }
            else
            {
                throw new ArgumentException("Argument distance error: rtrbau distance type not implemented.");
            }

            distanceType = distance;
        }
        #endregion CONSTRUCTOR

        #region ILOADABLE_METHODS
        public string URL()
        {
            return Parser.ParseOntDistURI(startClass, endClass);
        }

        public string FilePath()
        {
            string folder;

            if (Dictionaries.distanceDataDirectories.TryGetValue(distanceType, out folder)){}
            else { throw new ArgumentException("Argument distance error: rtrbau distance type not implemented."); }

            return folder + "/" + startClass.Entity() + ".json";
        }

        public string EventName()
        {
            return "Distance__" + distanceType + "__" + startClass.Entity();
        }

        #endregion ILOADABLE_METHODS
    }

    /// <summary>
    /// Describe script purpose
    /// Add links when code has been inspired
    /// </summary> 
    public class RtrbauFile : ILoadable
    {
        #region MEMBERS
        public string name;
        public RtrbauFileType type;
        public RtrbauAugmentation augmentation;
        private string url;
        #endregion MEMBERS

        #region CONSTRUCTOR
        public RtrbauFile()
        {
            name = null;
            type = RtrbauFileType.wav;
            augmentation = RtrbauAugmentation.Registration;
            url = null;
        }

        public RtrbauFile(string fileName, string fileType)
        {
            name = fileName;

            if (Enum.TryParse<RtrbauFileType>(fileType, out type)) {}
            else { throw new ArgumentException("Argument file error: file type not implemented."); }

            if (Dictionaries.FileAugmentations.TryGetValue(type, out augmentation)) { }
            else { throw new ArgumentException("Argument file error: file type not implement to an augmentation method."); }

            url = Parser.ParseFileURI(fileName, fileType);

        }

        public RtrbauFile(string filePath)
        {
            url = Parser.ParseURI(filePath, '/', RtrbauParser.pre) + "/";
            string file = Parser.ParseURI(filePath, '/', RtrbauParser.post);
            name = Parser.ParseURI(file, '.', RtrbauParser.pre);
            string fileType = Parser.ParseURI(file, '.', RtrbauParser.post);

            if (Enum.TryParse<RtrbauFileType>(fileType, out type)) {}
            else { throw new ArgumentException("Argument file error: file type not implemented."); }

            if (Dictionaries.FileAugmentations.TryGetValue(type, out augmentation)) { }
            else { throw new ArgumentException("Argument file error: file type not implement to an augmentation method."); }
        }
        #endregion CONSTRUCTOR

        #region ILOADABLE_METHODS
        public string URL()
        {
            return url + name + '.' + type.ToString();
        }

        public string FilePath()
        {
            string folder;

            if (Dictionaries.fileDataDirectories.TryGetValue(type, out folder)) { }
            else { throw new ArgumentException("Argument file error: file type not implemented."); }

            return folder + '/' + name + '.' + type.ToString();
        }

        public string EventName()
        {
            return "File__" + name + "__" + type.ToString();
        }
        #endregion ILOADABLE_METHODS
    }
    #endregion DATA_CLASSES
}
