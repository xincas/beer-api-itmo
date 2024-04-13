// Code generated by protoc-gen-go. DO NOT EDIT.
// versions:
// 	protoc-gen-go v1.33.0
// 	protoc        v5.26.1
// source: create_beer.proto

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

type CreateBeerRequest struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Beer *Beer `protobuf:"bytes,1,opt,name=beer,proto3" json:"beer,omitempty"`
}

func (x *CreateBeerRequest) Reset() {
	*x = CreateBeerRequest{}
	if protoimpl.UnsafeEnabled {
		mi := &file_create_beer_proto_msgTypes[0]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *CreateBeerRequest) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*CreateBeerRequest) ProtoMessage() {}

func (x *CreateBeerRequest) ProtoReflect() protoreflect.Message {
	mi := &file_create_beer_proto_msgTypes[0]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use CreateBeerRequest.ProtoReflect.Descriptor instead.
func (*CreateBeerRequest) Descriptor() ([]byte, []int) {
	return file_create_beer_proto_rawDescGZIP(), []int{0}
}

func (x *CreateBeerRequest) GetBeer() *Beer {
	if x != nil {
		return x.Beer
	}
	return nil
}

type CreateBeerResponse struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	BeerId int64 `protobuf:"varint,1,opt,name=beerId,proto3" json:"beerId,omitempty"`
}

func (x *CreateBeerResponse) Reset() {
	*x = CreateBeerResponse{}
	if protoimpl.UnsafeEnabled {
		mi := &file_create_beer_proto_msgTypes[1]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *CreateBeerResponse) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*CreateBeerResponse) ProtoMessage() {}

func (x *CreateBeerResponse) ProtoReflect() protoreflect.Message {
	mi := &file_create_beer_proto_msgTypes[1]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use CreateBeerResponse.ProtoReflect.Descriptor instead.
func (*CreateBeerResponse) Descriptor() ([]byte, []int) {
	return file_create_beer_proto_rawDescGZIP(), []int{1}
}

func (x *CreateBeerResponse) GetBeerId() int64 {
	if x != nil {
		return x.BeerId
	}
	return 0
}

var File_create_beer_proto protoreflect.FileDescriptor

var file_create_beer_proto_rawDesc = []byte{
	0x0a, 0x11, 0x63, 0x72, 0x65, 0x61, 0x74, 0x65, 0x5f, 0x62, 0x65, 0x65, 0x72, 0x2e, 0x70, 0x72,
	0x6f, 0x74, 0x6f, 0x12, 0x03, 0x61, 0x70, 0x69, 0x1a, 0x0a, 0x62, 0x65, 0x65, 0x72, 0x2e, 0x70,
	0x72, 0x6f, 0x74, 0x6f, 0x22, 0x32, 0x0a, 0x11, 0x43, 0x72, 0x65, 0x61, 0x74, 0x65, 0x42, 0x65,
	0x65, 0x72, 0x52, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x12, 0x1d, 0x0a, 0x04, 0x62, 0x65, 0x65,
	0x72, 0x18, 0x01, 0x20, 0x01, 0x28, 0x0b, 0x32, 0x09, 0x2e, 0x61, 0x70, 0x69, 0x2e, 0x42, 0x65,
	0x65, 0x72, 0x52, 0x04, 0x62, 0x65, 0x65, 0x72, 0x22, 0x2c, 0x0a, 0x12, 0x43, 0x72, 0x65, 0x61,
	0x74, 0x65, 0x42, 0x65, 0x65, 0x72, 0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x12, 0x16,
	0x0a, 0x06, 0x62, 0x65, 0x65, 0x72, 0x49, 0x64, 0x18, 0x01, 0x20, 0x01, 0x28, 0x03, 0x52, 0x06,
	0x62, 0x65, 0x65, 0x72, 0x49, 0x64, 0x42, 0x03, 0x5a, 0x01, 0x2e, 0x62, 0x06, 0x70, 0x72, 0x6f,
	0x74, 0x6f, 0x33,
}

var (
	file_create_beer_proto_rawDescOnce sync.Once
	file_create_beer_proto_rawDescData = file_create_beer_proto_rawDesc
)

func file_create_beer_proto_rawDescGZIP() []byte {
	file_create_beer_proto_rawDescOnce.Do(func() {
		file_create_beer_proto_rawDescData = protoimpl.X.CompressGZIP(file_create_beer_proto_rawDescData)
	})
	return file_create_beer_proto_rawDescData
}

var file_create_beer_proto_msgTypes = make([]protoimpl.MessageInfo, 2)
var file_create_beer_proto_goTypes = []interface{}{
	(*CreateBeerRequest)(nil),  // 0: api.CreateBeerRequest
	(*CreateBeerResponse)(nil), // 1: api.CreateBeerResponse
	(*Beer)(nil),               // 2: api.Beer
}
var file_create_beer_proto_depIdxs = []int32{
	2, // 0: api.CreateBeerRequest.beer:type_name -> api.Beer
	1, // [1:1] is the sub-list for method output_type
	1, // [1:1] is the sub-list for method input_type
	1, // [1:1] is the sub-list for extension type_name
	1, // [1:1] is the sub-list for extension extendee
	0, // [0:1] is the sub-list for field type_name
}

func init() { file_create_beer_proto_init() }
func file_create_beer_proto_init() {
	if File_create_beer_proto != nil {
		return
	}
	file_beer_proto_init()
	if !protoimpl.UnsafeEnabled {
		file_create_beer_proto_msgTypes[0].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*CreateBeerRequest); i {
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
		file_create_beer_proto_msgTypes[1].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*CreateBeerResponse); i {
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
	type x struct{}
	out := protoimpl.TypeBuilder{
		File: protoimpl.DescBuilder{
			GoPackagePath: reflect.TypeOf(x{}).PkgPath(),
			RawDescriptor: file_create_beer_proto_rawDesc,
			NumEnums:      0,
			NumMessages:   2,
			NumExtensions: 0,
			NumServices:   0,
		},
		GoTypes:           file_create_beer_proto_goTypes,
		DependencyIndexes: file_create_beer_proto_depIdxs,
		MessageInfos:      file_create_beer_proto_msgTypes,
	}.Build()
	File_create_beer_proto = out.File
	file_create_beer_proto_rawDesc = nil
	file_create_beer_proto_goTypes = nil
	file_create_beer_proto_depIdxs = nil
}
