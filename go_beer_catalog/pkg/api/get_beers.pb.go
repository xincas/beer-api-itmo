// Code generated by protoc-gen-go. DO NOT EDIT.
// versions:
// 	protoc-gen-go v1.33.0
// 	protoc        v5.26.1
// source: get_beers.proto

package __

import (
	protoreflect "google.golang.org/protobuf/reflect/protoreflect"
	protoimpl "google.golang.org/protobuf/runtime/protoimpl"
	reflect "reflect"
	sync "sync"
)

const (
	// Verify that this generated code is sufficiently up-to-date.
	_ = protoimpl.EnforceVersion(20 - protoimpl.MinVersion)
	// Verify that runtime/protoimpl is sufficiently up-to-date.
	_ = protoimpl.EnforceVersion(protoimpl.MaxVersion - 20)
)

type GetBeersRequest struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Limit *int32  `protobuf:"varint,1,opt,name=limit,proto3,oneof" json:"limit,omitempty"`
	Name  *string `protobuf:"bytes,2,opt,name=name,proto3,oneof" json:"name,omitempty"`
	Brand *string `protobuf:"bytes,3,opt,name=brand,proto3,oneof" json:"brand,omitempty"`
	Type  *Type   `protobuf:"varint,4,opt,name=type,proto3,enum=api.Type,oneof" json:"type,omitempty"`
	Deg   *int32  `protobuf:"varint,5,opt,name=deg,proto3,oneof" json:"deg,omitempty"`
	Sweet *bool   `protobuf:"varint,6,opt,name=sweet,proto3,oneof" json:"sweet,omitempty"`
}

func (x *GetBeersRequest) Reset() {
	*x = GetBeersRequest{}
	if protoimpl.UnsafeEnabled {
		mi := &file_get_beers_proto_msgTypes[0]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *GetBeersRequest) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*GetBeersRequest) ProtoMessage() {}

func (x *GetBeersRequest) ProtoReflect() protoreflect.Message {
	mi := &file_get_beers_proto_msgTypes[0]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use GetBeersRequest.ProtoReflect.Descriptor instead.
func (*GetBeersRequest) Descriptor() ([]byte, []int) {
	return file_get_beers_proto_rawDescGZIP(), []int{0}
}

func (x *GetBeersRequest) GetLimit() int32 {
	if x != nil && x.Limit != nil {
		return *x.Limit
	}
	return 0
}

func (x *GetBeersRequest) GetName() string {
	if x != nil && x.Name != nil {
		return *x.Name
	}
	return ""
}

func (x *GetBeersRequest) GetBrand() string {
	if x != nil && x.Brand != nil {
		return *x.Brand
	}
	return ""
}

func (x *GetBeersRequest) GetType() Type {
	if x != nil && x.Type != nil {
		return *x.Type
	}
	return Type_ALE
}

func (x *GetBeersRequest) GetDeg() int32 {
	if x != nil && x.Deg != nil {
		return *x.Deg
	}
	return 0
}

func (x *GetBeersRequest) GetSweet() bool {
	if x != nil && x.Sweet != nil {
		return *x.Sweet
	}
	return false
}

type GetBeersResponse struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Beers []*Beer `protobuf:"bytes,1,rep,name=beers,proto3" json:"beers,omitempty"`
}

func (x *GetBeersResponse) Reset() {
	*x = GetBeersResponse{}
	if protoimpl.UnsafeEnabled {
		mi := &file_get_beers_proto_msgTypes[1]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *GetBeersResponse) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*GetBeersResponse) ProtoMessage() {}

func (x *GetBeersResponse) ProtoReflect() protoreflect.Message {
	mi := &file_get_beers_proto_msgTypes[1]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use GetBeersResponse.ProtoReflect.Descriptor instead.
func (*GetBeersResponse) Descriptor() ([]byte, []int) {
	return file_get_beers_proto_rawDescGZIP(), []int{1}
}

func (x *GetBeersResponse) GetBeers() []*Beer {
	if x != nil {
		return x.Beers
	}
	return nil
}

var File_get_beers_proto protoreflect.FileDescriptor

var file_get_beers_proto_rawDesc = []byte{
	0x0a, 0x0f, 0x67, 0x65, 0x74, 0x5f, 0x62, 0x65, 0x65, 0x72, 0x73, 0x2e, 0x70, 0x72, 0x6f, 0x74,
	0x6f, 0x12, 0x03, 0x61, 0x70, 0x69, 0x1a, 0x0a, 0x62, 0x65, 0x65, 0x72, 0x2e, 0x70, 0x72, 0x6f,
	0x74, 0x6f, 0x22, 0xee, 0x01, 0x0a, 0x0f, 0x47, 0x65, 0x74, 0x42, 0x65, 0x65, 0x72, 0x73, 0x52,
	0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x12, 0x19, 0x0a, 0x05, 0x6c, 0x69, 0x6d, 0x69, 0x74, 0x18,
	0x01, 0x20, 0x01, 0x28, 0x05, 0x48, 0x00, 0x52, 0x05, 0x6c, 0x69, 0x6d, 0x69, 0x74, 0x88, 0x01,
	0x01, 0x12, 0x17, 0x0a, 0x04, 0x6e, 0x61, 0x6d, 0x65, 0x18, 0x02, 0x20, 0x01, 0x28, 0x09, 0x48,
	0x01, 0x52, 0x04, 0x6e, 0x61, 0x6d, 0x65, 0x88, 0x01, 0x01, 0x12, 0x19, 0x0a, 0x05, 0x62, 0x72,
	0x61, 0x6e, 0x64, 0x18, 0x03, 0x20, 0x01, 0x28, 0x09, 0x48, 0x02, 0x52, 0x05, 0x62, 0x72, 0x61,
	0x6e, 0x64, 0x88, 0x01, 0x01, 0x12, 0x22, 0x0a, 0x04, 0x74, 0x79, 0x70, 0x65, 0x18, 0x04, 0x20,
	0x01, 0x28, 0x0e, 0x32, 0x09, 0x2e, 0x61, 0x70, 0x69, 0x2e, 0x54, 0x79, 0x70, 0x65, 0x48, 0x03,
	0x52, 0x04, 0x74, 0x79, 0x70, 0x65, 0x88, 0x01, 0x01, 0x12, 0x15, 0x0a, 0x03, 0x64, 0x65, 0x67,
	0x18, 0x05, 0x20, 0x01, 0x28, 0x05, 0x48, 0x04, 0x52, 0x03, 0x64, 0x65, 0x67, 0x88, 0x01, 0x01,
	0x12, 0x19, 0x0a, 0x05, 0x73, 0x77, 0x65, 0x65, 0x74, 0x18, 0x06, 0x20, 0x01, 0x28, 0x08, 0x48,
	0x05, 0x52, 0x05, 0x73, 0x77, 0x65, 0x65, 0x74, 0x88, 0x01, 0x01, 0x42, 0x08, 0x0a, 0x06, 0x5f,
	0x6c, 0x69, 0x6d, 0x69, 0x74, 0x42, 0x07, 0x0a, 0x05, 0x5f, 0x6e, 0x61, 0x6d, 0x65, 0x42, 0x08,
	0x0a, 0x06, 0x5f, 0x62, 0x72, 0x61, 0x6e, 0x64, 0x42, 0x07, 0x0a, 0x05, 0x5f, 0x74, 0x79, 0x70,
	0x65, 0x42, 0x06, 0x0a, 0x04, 0x5f, 0x64, 0x65, 0x67, 0x42, 0x08, 0x0a, 0x06, 0x5f, 0x73, 0x77,
	0x65, 0x65, 0x74, 0x22, 0x33, 0x0a, 0x10, 0x47, 0x65, 0x74, 0x42, 0x65, 0x65, 0x72, 0x73, 0x52,
	0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x12, 0x1f, 0x0a, 0x05, 0x62, 0x65, 0x65, 0x72, 0x73,
	0x18, 0x01, 0x20, 0x03, 0x28, 0x0b, 0x32, 0x09, 0x2e, 0x61, 0x70, 0x69, 0x2e, 0x42, 0x65, 0x65,
	0x72, 0x52, 0x05, 0x62, 0x65, 0x65, 0x72, 0x73, 0x42, 0x03, 0x5a, 0x01, 0x2e, 0x62, 0x06, 0x70,
	0x72, 0x6f, 0x74, 0x6f, 0x33,
}

var (
	file_get_beers_proto_rawDescOnce sync.Once
	file_get_beers_proto_rawDescData = file_get_beers_proto_rawDesc
)

func file_get_beers_proto_rawDescGZIP() []byte {
	file_get_beers_proto_rawDescOnce.Do(func() {
		file_get_beers_proto_rawDescData = protoimpl.X.CompressGZIP(file_get_beers_proto_rawDescData)
	})
	return file_get_beers_proto_rawDescData
}

var file_get_beers_proto_msgTypes = make([]protoimpl.MessageInfo, 2)
var file_get_beers_proto_goTypes = []interface{}{
	(*GetBeersRequest)(nil),  // 0: api.GetBeersRequest
	(*GetBeersResponse)(nil), // 1: api.GetBeersResponse
	(Type)(0),                // 2: api.Type
	(*Beer)(nil),             // 3: api.Beer
}
var file_get_beers_proto_depIdxs = []int32{
	2, // 0: api.GetBeersRequest.type:type_name -> api.Type
	3, // 1: api.GetBeersResponse.beers:type_name -> api.Beer
	2, // [2:2] is the sub-list for method output_type
	2, // [2:2] is the sub-list for method input_type
	2, // [2:2] is the sub-list for extension type_name
	2, // [2:2] is the sub-list for extension extendee
	0, // [0:2] is the sub-list for field type_name
}

func init() { file_get_beers_proto_init() }
func file_get_beers_proto_init() {
	if File_get_beers_proto != nil {
		return
	}
	file_beer_proto_init()
	if !protoimpl.UnsafeEnabled {
		file_get_beers_proto_msgTypes[0].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*GetBeersRequest); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_get_beers_proto_msgTypes[1].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*GetBeersResponse); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
	}
	file_get_beers_proto_msgTypes[0].OneofWrappers = []interface{}{}
	type x struct{}
	out := protoimpl.TypeBuilder{
		File: protoimpl.DescBuilder{
			GoPackagePath: reflect.TypeOf(x{}).PkgPath(),
			RawDescriptor: file_get_beers_proto_rawDesc,
			NumEnums:      0,
			NumMessages:   2,
			NumExtensions: 0,
			NumServices:   0,
		},
		GoTypes:           file_get_beers_proto_goTypes,
		DependencyIndexes: file_get_beers_proto_depIdxs,
		MessageInfos:      file_get_beers_proto_msgTypes,
	}.Build()
	File_get_beers_proto = out.File
	file_get_beers_proto_rawDesc = nil
	file_get_beers_proto_goTypes = nil
	file_get_beers_proto_depIdxs = nil
}
