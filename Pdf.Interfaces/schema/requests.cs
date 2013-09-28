﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5472
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.1432.
// 
namespace Pdf.Interfaces {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:pdfstamper:handlers")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:pdfstamper:handlers", IsNullable=false)]
    public partial class pdf_stamper_request {
        
        private template[] pdf_template_listField;
        
        private string dataField;
        
        private bool read_onlyField;
        
        private bool debug_modeField;
        
        private bool merge_outputField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("pdf_template_list")]
        public template[] pdf_template_list {
            get {
                return this.pdf_template_listField;
            }
            set {
                this.pdf_template_listField = value;
            }
        }
        
        /// <remarks/>
        public string data {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
            }
        }
        
        /// <remarks/>
        public bool read_only {
            get {
                return this.read_onlyField;
            }
            set {
                this.read_onlyField = value;
            }
        }
        
        /// <remarks/>
        public bool debug_mode {
            get {
                return this.debug_modeField;
            }
            set {
                this.debug_modeField = value;
            }
        }
        
        /// <remarks/>
        public bool merge_output {
            get {
                return this.merge_outputField;
            }
            set {
                this.merge_outputField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:pdfstamper:handlers")]
    public enum template {
        
        /// <remarks/>
        Obrazac1,
        
        /// <remarks/>
        Obrazac2,
        
        /// <remarks/>
        Obrazac3,
        
        /// <remarks/>
        Obrazac4,
        
        /// <remarks/>
        Obrazac5,
        
        /// <remarks/>
        Obrazac6,
        
        /// <remarks/>
        Obrazac7,
        
        /// <remarks/>
        Obrazac8,
        
        /// <remarks/>
        Obrazac9,
        
        /// <remarks/>
        Obrazac10,
        
        /// <remarks/>
        Obrazac11,
        
        /// <remarks/>
        Obrazac12,
        
        /// <remarks/>
        Obrazac13,
        
        /// <remarks/>
        Obrazac14,
        
        /// <remarks/>
        Obrazac15,
        
        /// <remarks/>
        Obrazac16,
        
        /// <remarks/>
        Obrazac17,
        
        /// <remarks/>
        Obrazac18,
        
        /// <remarks/>
        Obrazac19,
        
        /// <remarks/>
        Obrazac20,
        
        /// <remarks/>
        Obrazac21,
        
        /// <remarks/>
        Obrazac22,
        
        /// <remarks/>
        Obrazac23,
        
        /// <remarks/>
        Obrazac24,
        
        /// <remarks/>
        Obrazac25,
        
        /// <remarks/>
        Obrazac26,
        
        /// <remarks/>
        Obrazac27,
        
        /// <remarks/>
        Obrazac28,
        
        /// <remarks/>
        Obrazac29,
        
        /// <remarks/>
        Obrazac30,
        
        /// <remarks/>
        MergedContent,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:pdfstamper:handlers")]
    public partial class mapping_type {
        
        private string column_nameField;
        
        private string data_fieldField;
        
        private data_field_type field_typeField;
        
        /// <remarks/>
        public string column_name {
            get {
                return this.column_nameField;
            }
            set {
                this.column_nameField = value;
            }
        }
        
        /// <remarks/>
        public string data_field {
            get {
                return this.data_fieldField;
            }
            set {
                this.data_fieldField = value;
            }
        }
        
        /// <remarks/>
        public data_field_type field_type {
            get {
                return this.field_typeField;
            }
            set {
                this.field_typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:pdfstamper:handlers")]
    public enum data_field_type {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Check Box")]
        CheckBox,
        
        /// <remarks/>
        Text,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Radio Button")]
        RadioButton,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:pdfstamper:handlers")]
    public partial class mapping_item_type {
        
        private template pdf_templateField;
        
        private string file_pathField;
        
        private mapping_type[] mappingField;
        
        /// <remarks/>
        public template pdf_template {
            get {
                return this.pdf_templateField;
            }
            set {
                this.pdf_templateField = value;
            }
        }
        
        /// <remarks/>
        public string file_path {
            get {
                return this.file_pathField;
            }
            set {
                this.file_pathField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("mapping")]
        public mapping_type[] mapping {
            get {
                return this.mappingField;
            }
            set {
                this.mappingField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:pdfstamper:handlers")]
    public partial class response_item_type {
        
        private string dataField;
        
        private template pdf_templateField;
        
        /// <remarks/>
        public string data {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
            }
        }
        
        /// <remarks/>
        public template pdf_template {
            get {
                return this.pdf_templateField;
            }
            set {
                this.pdf_templateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:pdfstamper:handlers")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:pdfstamper:handlers", IsNullable=false)]
    public partial class pdf_stamper_response {
        
        private response_item_type[] response_itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("response_items")]
        public response_item_type[] response_items {
            get {
                return this.response_itemsField;
            }
            set {
                this.response_itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:pdfstamper:handlers")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:pdfstamper:handlers", IsNullable=false)]
    public partial class mappings {
        
        private mapping_item_type[] mapping_itemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("mapping_item")]
        public mapping_item_type[] mapping_item {
            get {
                return this.mapping_itemField;
            }
            set {
                this.mapping_itemField = value;
            }
        }
    }
}
