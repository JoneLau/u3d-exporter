# /////////////////////////////////////////////////////////////////////////////
#  settings
# /////////////////////////////////////////////////////////////////////////////

# settings (mac)
unity_path = /Applications/Unity/Unity.app/Contents/

# unit essential (mac)
compiler = $(unity_path)/Mono/bin/gmcs

unity_engine_dll = $(unity_path)/Managed/UnityEngine.dll
unity_engine_ui_dll = $(unity_path)/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll
unity_editor_dll = $(unity_path)/Managed/UnityEditor.dll
json_dll = ./Assets/vendors/JsonNet-Lite/Newtonsoft.Json.dll

# Utilities.
MKDIR = mkdir -p
RM = rm -f
RMDIR = rm -rf

# Target
dest = ./bin
prepare = $(dest)
runtime_target = $(dest)/u3d-exporter.runtime.dll
editor_target = $(dest)/Editor/u3d-exporter.dll

# /////////////////////////////////////////////////////////////////////////////
# builds
# /////////////////////////////////////////////////////////////////////////////

.PHONY: clean rebuild

all: $(prepare) $(runtime_target) $(editor_target)
clean:
	$(RMDIR) $(dest)
rebuild: |clean all

# defines
editor_define = UNITY_EDITOR

# /////////////////////////////////////////////////////////////////////////////
# targets
# /////////////////////////////////////////////////////////////////////////////

# get sources
runtime_src_dirs += ./Assets/Scripts/
runtime_src = $(wildcard $(addsuffix *.cs,$(runtime_src_dirs)))

editor_src_dirs += ./Assets/Editor/
editor_src = $(wildcard $(addsuffix *.cs,$(editor_src_dirs)))

# resources
# resources += -resource:Assets/Resource/pixel.png

# argument
runtime_args = $(resources) -r:$(unity_engine_dll),$(unity_engine_ui_dll)
editor_args = $(resources) -d:$(editor_define) -r:$(unity_engine_dll),$(unity_engine_ui_dll),$(unity_editor_dll),$(json_dll),$(runtime_target)

# do the build
$(prepare):
	@echo "========================================================"
	@echo "copy shaders & vendors"
	@echo "========================================================"
	@echo
	$(MKDIR) $(dest)
	$(MKDIR) $(dest)/Editor
	cp -r ./Assets/shaders/ $(dest)/shaders/
	cp -r ./Assets/vendors/ $(dest)/vendors/
	@echo done!
	@echo

$(runtime_target):
	@echo "========================================================"
	@echo "compiling u3d-exporter.runtime.dll..."
	@echo "========================================================"
	@echo
	$(compiler) -target:library -out:$(runtime_target) $(runtime_args) $(runtime_src)
	@echo done!
	@echo

$(editor_target):
	@echo "========================================================"
	@echo "compiling Editor/u3d-exporter.dll..."
	@echo "========================================================"
	@echo
	$(compiler) -target:library -out:$(editor_target) $(editor_args) $(editor_src)
	@echo done!
	@echo