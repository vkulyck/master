function OnPropertyChanged(handler, wrapper, newValue) {
    return wrapper.invokeMethodAsync(handler, newValue);
}